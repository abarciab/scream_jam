using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Eel : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] Transform patrolPointParent;
    List<Transform> patrolPoints = new List<Transform>();
    [SerializeField] bool inOrder, loop = true, patrolOnStart = true, patrolWhenChill, pingPong = true;
    int pointsLeft;
    bool patrolling;
    [SerializeField] bool debugPoints;
    int currentPointIndex;

    [Header("Misc")]
    [SerializeField] float turnSmoothness = 0.05f;
    [SerializeField] float moveSpeed, targetThreshold = 1.5f, moveSmoothness;
    Vector3 currentTarget, posDelta;
    public bool scriptedBehavior, updateRotation = true, goToTarget = true;

    [Header("Aggro")]
    [SerializeField] bool aggro;
    [SerializeField] bool behindAttack, frontAttack;
    [SerializeField] float attackResetTime = 3, attackDamage, chargeSpeed, chargeForce, backAwayDist;
    float attackCooldown;
    Player player;

    [Header("ColorChange")]
    [SerializeField] Renderer eyeRenderer;
    [SerializeField] Material calmMat, aggroMat;

    [Header("Anims")]
    [SerializeField] Animator anim;
    [SerializeField] string biteTrigger = "bite";
    [SerializeField] float distanceFromPlayerWhenBite = 20;
    bool startedBiteAnim;

    Enemy enemyScript;
    public float distanceToTarget { get { return Vector3.Distance(transform.position, currentTarget); } }
    public bool atTarget { get { return distanceToTarget < 3f; } }
    [HideInInspector] public bool fastTurn;

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
        if (patrolOnStart) StartPatrol();
    }

    public void StartPatrol()
    {
        if (patrolPoints.Count == 0) return;
        currentPointIndex = 0;
        pointsLeft = patrolPoints.Count;
        patrolling = true;
        NextPoint();
    }

    void NextPoint()
    {
        if (pointsLeft == 0) {
            ReversePatrolPointOrder();
            EndPatrol();
            return;
        }

        currentTarget = patrolPoints[currentPointIndex].position;
        currentPointIndex += 1;
        pointsLeft -= 1;
    }

    void ReversePatrolPointOrder()
    {
        patrolPoints.Reverse();
    }

    void EndPatrol()
    {
        patrolling = false;
        if (loop) StartPatrol();
    }

    private void Update()
    {
        eyeRenderer.material = aggro ? aggroMat : calmMat;

        if (enemyScript.aggro) aggro = true;
        if (scriptedBehavior) {
            GoToCurrentTarget();
            return;
        }
        else if (!enemyScript.aggro && !enemyScript.alert && !patrolling) StartPatrol();

        attackCooldown -= Time.deltaTime;
        if (aggro) ChasePlayer();
        else if (patrolling) GoToNextPoint();

        GoToCurrentTarget();
    }

    void ChasePlayer()
    {
        if (attackCooldown <= 0) MoveToAttack();
    }

    void CompleteAttack()
    {
        enemyScript.EnterCalm();
        aggro = false;
        PickNewAttack();
    }

    void MoveToAttack()
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

        CompleteAttack();
    }

    void PickNewAttack()
    {
        frontAttack = true;
        behindAttack = false;
    }

    void GoToNextPoint()
    {
        float dist = Vector3.Distance(transform.position, currentTarget);
        if (dist < targetThreshold) NextPoint();
    }

    void GoToCurrentTarget()
    {
        var targetPos = Vector3.zero;
        var rot = transform.rotation;

        if (!atTarget) {
            if (updateRotation) transform.LookAt(currentTarget);
            if (goToTarget) targetPos = moveSpeed * transform.forward;
        }
        transform.rotation = Quaternion.Lerp(rot, transform.rotation, fastTurn ? 1 : turnSmoothness);
        posDelta = Vector3.Lerp(posDelta, targetPos, moveSmoothness);
        transform.position += posDelta * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, currentTarget);
        Handles.Label(transform.position, (Mathf.Round(Vector3.Distance(transform.position, currentTarget) * 100) / 100).ToString());

        if (!debugPoints) return;
        if (!Application.isPlaying && patrolPointParent && patrolPointParent.childCount != patrolPoints.Count) Init();

        for (int i = 0; i < patrolPoints.Count; i++) {
            if (i == patrolPoints.Count - 1) break;
            var next = i + 1;
            Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[next].position);
            Gizmos.DrawWireSphere(patrolPoints[i].position, targetThreshold);
        }
    }

    private void OnDestroy()
    {
        EnvironmentManager.current.RemoveMonster(transform);
    }
}
