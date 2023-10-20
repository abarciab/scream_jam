using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class SharkController : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] Transform patrolPointParent;
    List<Transform> patrolPoints = new List<Transform>();
    [SerializeField] bool inOrder, loop = true, patrolOnStart = true, patrolWhenChill;
    int pointsLeft;
    bool patrolling;
    [SerializeField] bool debugPoints;

    [Header("Misc")]
    [SerializeField] float turnSmoothness = 0.05f;
    [SerializeField] float moveSpeed, targetThreshold = 1.5f, moveSmoothness;
    Vector3 currentTarget, posDelta;
    public bool scriptedBehavior, updateRotation = true, goToTarget = true;

    [Header("Aggro")]
    [SerializeField] bool aggro;
    [SerializeField] bool behindAttack, frontAttack;
    [SerializeField] float backAwayDist = 4, attackResetTime = 3, attackDamage, frontAttackChargeDist = 50, chargeSpeed, chargeForce;
    float attackCooldown;
    bool charging;
    Player player;

    [Header("ColorChange")]
    [SerializeField] List<Renderer> indicatorRenderers = new List<Renderer>();
    [SerializeField] Renderer bodyRenderer;
    [SerializeField] Material calmMat, aggroMat;

    [Header("Anims")]
    [SerializeField] Animator anim;
    [SerializeField] string biteTrigger = "bite";

    Enemy enemyScript;
    public float distanceToTarget { get { return Vector3.Distance(transform.position, currentTarget); } }
    bool liningUpFrontAttack { get { return aggro && frontAttack && attackCooldown <= 0 && !charging; } }
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
        var mat = aggro ? aggroMat : calmMat;
        foreach (var r in indicatorRenderers) r.material = mat;
        var mats = bodyRenderer.materials;
        mats[1] = mat;
        bodyRenderer.materials = mats;

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
        transform.position = player.transform.position + 3 * backAwayDist * dir;
        attackCooldown = attackResetTime;
        currentTarget = transform.position;
        charging = false;

        CompleteAttack();
    }

    void CompleteAttack()
    {
        enemyScript.EnterCalm();
        aggro = false;
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
            if (updateRotation) {
                transform.LookAt(currentTarget);
                if (liningUpFrontAttack) {
                    var posDifference = currentTarget - transform.position;
                    transform.LookAt(transform.position - posDifference);
                }
            }
            if (goToTarget) targetPos = (charging ? chargeSpeed : moveSpeed) * (liningUpFrontAttack ? -1 : 1) * transform.forward;
        }

        transform.rotation = Quaternion.Lerp(rot, transform.rotation, fastTurn ? 1 : turnSmoothness);
        posDelta = Vector3.Lerp(posDelta, targetPos, moveSmoothness);
        transform.position += posDelta * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, currentTarget);
        Handles.Label(transform.position, (Mathf.Round(Vector3.Distance(transform.position, currentTarget) * 100)/100).ToString());
        
        if (!debugPoints) return;
        if (!Application.isPlaying && patrolPointParent && patrolPointParent.childCount != patrolPoints.Count) Init();

        for (int i = 0; i < patrolPoints.Count; i++) {
            int next = i == patrolPoints.Count - 1 ? 0 : i + 1;
            Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[next].position);
            Gizmos.DrawWireSphere(patrolPoints[i].position, targetThreshold);
        }
    }

    private void OnDestroy()
    {
        EnvironmentManager.current.RemoveMonster(transform);
    }
}
