using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField] Transform model;

    [Header("FishWiggle")]
    [SerializeField] float frequency;
    [SerializeField] float amplitude, fishOffset, lookSmoothness = 0.1f;
    Vector3 oldPos, lookDir;

    [Header("Patrolling")]
    [SerializeField] Transform patrolPointParent;
    [SerializeField] float pointDistThreshold, turnSmoothness = 0.05f, moveSpeed = 3;
    [SerializeField] int currentPoint;
    [SerializeField] bool drawGizmos;
    Vector3 currentTarget;

    [Header("Randomness")]
    [SerializeField, ReadOnly] float startOffset;
    [SerializeField] float randomPointSizeRange;
    [SerializeField] Gradient colorGradient;
    [SerializeField] Renderer skinRenderer;

    void SetSeed()
    {
        startOffset = Random.Range(0.0f, 1000);
        float colorChoice = startOffset / 1000;
        skinRenderer.material.color = colorGradient.Evaluate(colorChoice);
    }

    private void Start()
    {
        SetSeed();
        PickNewPoint();
    }

    private void Update()
    {
        float fishY = amplitude * Mathf.Sin(frequency * (Time.time + fishOffset + startOffset));
        model.localPosition = new Vector3(model.localPosition.x, fishY, model.localPosition.z);

        var dir = model.position - oldPos;
        oldPos = model.position;
        lookDir = Vector3.Lerp(lookDir, dir.normalized, lookSmoothness);
        model.LookAt(model.transform.position + lookDir); 

        GoToNextPoint();
    }

    void GoToNextPoint()
    {
        float dist = Vector3.Distance(transform.position, currentTarget);
        if (dist < pointDistThreshold) {
            PickNewPoint();
            return;
        }

        var startRot = transform.localEulerAngles;
        transform.LookAt(currentTarget);
        var targetRot = transform.localEulerAngles;
        transform.localRotation = Quaternion.Lerp(Quaternion.Euler(startRot), Quaternion.Euler(targetRot), turnSmoothness);

        var dir = transform.forward * moveSpeed * Time.deltaTime;
        transform.position += dir;
    }

    void PickNewPoint()
    {
        currentTarget = patrolPointParent.GetChild(currentPoint).position;
        currentTarget += Random.onUnitSphere * randomPointSizeRange;

        currentPoint += 1;
        if (currentPoint >= patrolPointParent.childCount) currentPoint = 0;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos || patrolPointParent == null) return;

        var patrolPoints = new List<Transform>();
        foreach (Transform child in patrolPointParent) patrolPoints.Add(child);
        for (int i = 0; i < patrolPoints.Count; i++) {
            int next = i == patrolPoints.Count - 1 ? 0 : i + 1;
            Gizmos.color = Color.white;
            Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[next].position);
            Gizmos.DrawWireSphere(patrolPoints[i].position, pointDistThreshold);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(patrolPoints[i].position, randomPointSizeRange);
        }
    }

}
