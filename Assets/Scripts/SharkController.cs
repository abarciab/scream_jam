using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
    public bool scriptedBehavior;

    [Header("Aggro")]
    [SerializeField] bool aggro;
    [SerializeField] bool behindAttack, frontAttack;
    [SerializeField] float backAwayDist = 4, attackResetTime = 3, attackDamage, frontAttackChargeDist = 50, chargeSpeed, chargeForce;
    float attackCooldown;
    bool charging;
    Player player;

    Enemy enemyScript;
    public float distanceToTarget { get { return Vector3.Distance(transform.position, currentTarget); } }

    private void OnValidate()
    {
        if (patrolPointParent != null) Init();
    }

    public void SetTarget(Vector3 newTarget)
    {
        currentTarget = newTarget;
    }

    void Init()
    {
        patrolPoints.Clear();
        foreach (Transform child in patrolPointParent) patrolPoints.Add(child);
    }

    private void Start()
    {
        enemyScript = GetComponent<Enemy>();
        player = PlayerManager.i.player;
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
        if (scriptedBehavior) {
            GoToCurrentTarget();
            return;
        }

        if (enemyScript.aggro) aggro = true;
        attackCooldown -= Time.deltaTime;
        if (aggro) ChasePlayer();
        else if (patrolling) GoToNextPoint();

        GoToCurrentTarget();
    }

    void ChasePlayer()
    {
        if (attackCooldown > 0) return;
        if (behindAttack) SwimToBackAttack();
        else if (frontAttack) SwimToFrontAttack();
    }

    void SwimToFrontAttack()
    {
        if (charging) currentTarget = PlayerManager.i.submarine.position;
        else SwimInFrontOfPlayer();

        if (charging && player.enemiesInContact.Contains(transform)) AttackfromFront();
    }

    void AttackfromFront()
    {
        player.ShakeSub();
        player.Damage(attackDamage);
        var dir = transform.position - player.transform.position;
        player.PushSub(dir * -chargeForce);
        transform.position = player.transform.position -3 * backAwayDist * dir;
        attackCooldown = attackResetTime;
        currentTarget = transform.position;
        charging = false;

        PickNewAttack();
    }

    void SwimInFrontOfPlayer()
    {
        currentTarget = player.transform.position + player.transform.forward * frontAttackChargeDist;
        if (Vector3.Distance(transform.position, currentTarget) < targetThreshold) charging = true;
    }

    void SwimToBackAttack()
    {
        currentTarget = PlayerManager.i.submarine.forward * -1;
        if (player.enemiesInTrigger.Contains(transform)) AttackFromBehind();
    }

    void AttackFromBehind()
    {
        player.ShakeSub();
        player.Damage(attackDamage);
        var dir = transform.position - player.transform.position;
        transform.position = player.transform.position + dir * backAwayDist;
        attackCooldown = attackResetTime;
        currentTarget = transform.position;

        PickNewAttack();
    }

    void PickNewAttack()
    {
        frontAttack = !frontAttack;
        behindAttack = !behindAttack; 
    }

    void GoToNextPoint()
    {
        float dist = Vector3.Distance(transform.position, currentTarget);
        if (dist < targetThreshold) NextPoint();
    }

    void GoToCurrentTarget()
    {
        if (Vector3.Distance(transform.position, currentTarget) < targetThreshold * 0.75f) return;

        var rot = transform.rotation;
        transform.LookAt(currentTarget);
        transform.rotation = Quaternion.Lerp(rot, transform.rotation, turnSmoothness);
        transform.position += transform.forward * (charging ? chargeSpeed : moveSpeed) * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        if (!debugPoints) return;
        if (!Application.isPlaying && patrolPointParent && patrolPointParent.childCount != patrolPoints.Count) Init();

        for (int i = 0; i < patrolPoints.Count; i++) {
            int next = i == patrolPoints.Count - 1 ? 0 : i + 1;
            Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[next].position);
            Gizmos.DrawWireSphere(patrolPoints[i].position, targetThreshold);
        }
    }
}
