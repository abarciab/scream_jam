using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkController : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] Transform patrolPointParent;
    List<Transform> patrolPoints = new List<Transform>();
    [SerializeField] bool inOrder, loop = true, patrolOnStart = true;
    int pointsLeft;
    bool patrolling;
    [SerializeField] bool debugPoints;

    [Header("Misc")]
    [SerializeField] float turnSmoothness = 0.05f;
    [SerializeField] float moveSpeed, targetThreshold = 1.5f;
    Vector3 currentTarget;

    private void OnValidate()
    {
        if (patrolPointParent != null) Init();
    }

    void Init()
    {

        patrolPoints.Clear();
        foreach (Transform child in patrolPointParent) patrolPoints.Add(child);
    }

    private void Start()
    {
        EnvironmentManager.current.RegisterNewMonster(transform);
        if (patrolOnStart) StartPatrol();
    }

    public void StartPatrol()
    {
        if (patrolPoints.Count ==0) return;
        pointsLeft = patrolPoints.Count;
        patrolling = true;
        NextPoint();
    }

    void NextPoint()
    {
        if (pointsLeft == 0) {
            EndPatrol();
            return;
        }

        int index = inOrder ? 0 : Random.Range(0, patrolPoints.Count);
        currentTarget = patrolPoints[index].position;
        patrolPoints.Add(patrolPoints[index]);
        patrolPoints.RemoveAt(index);
        pointsLeft -= 1;
    }

    void EndPatrol()
    {
        patrolling = false;
        if (loop) StartPatrol();
    }

    private void Update()
    {
        if (patrolling) GoToNextPoint();
    }

    void GoToNextPoint()
    {
        var rot = transform.rotation;
        transform.LookAt(currentTarget);
        transform.rotation = Quaternion.Lerp(rot, transform.rotation, turnSmoothness);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        float dist = Vector3.Distance(transform.position, currentTarget);
        if (dist < targetThreshold) NextPoint();
    }

    private void OnDrawGizmos()
    {
        if (!debugPoints) return;
        if (!Application.isPlaying && patrolPointParent.childCount != patrolPoints.Count) Init();

        for (int i = 0; i < patrolPoints.Count; i++) {
            int next = i == patrolPoints.Count - 1 ? 0 : i + 1;
            Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[next].position);
            Gizmos.DrawWireSphere(patrolPoints[i].position, targetThreshold);
        }
    }
}
