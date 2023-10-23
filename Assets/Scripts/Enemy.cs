using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    [SerializeField] Sound Scream;
    [SerializeField] Vector2 screamWaitRange = new Vector2(5, 20);
    float screamCooldown;
    [SerializeField] float hearRange = 20, soundThreshold = 0.7f, alertToAgroTime = 8, InvestigationTime = 5;
    public bool aggro, alert;
    float timeAlert;
    [SerializeField] LayerMask raycastMask;

    [SerializeField] List<Vector3> lightCheckPoints = new List<Vector3>();

    [Header("Behavior")]
    [SerializeField] float attackCalmTime = 3;
    [SerializeField] float postCalmAlertTime = 5;
    float postAttackCalmCooldown, alertCooldown;
    SharkController shark;

    private void Start()
    {
        EnvironmentManager.current.RegisterNewMonster(transform);
        Scream = Instantiate(Scream);
        screamCooldown = Random.Range(screamWaitRange.x, screamWaitRange.y);
        shark = GetComponent<SharkController>();
    }

    public void EnterPostAttackCalm()
    {
        postAttackCalmCooldown = attackCalmTime;
        aggro = false;
        alert = true;
    }

    private void Update()
    {
        TryScream();
        checkCooldowns();
        CheckDetect();
    }

    void checkCooldowns()
    {
        if (postAttackCalmCooldown > 0) {
            timeAlert = 0;
            postAttackCalmCooldown -= Time.deltaTime;
            transform.LookAt(PlayerManager.i.submarine.position);
            if (postAttackCalmCooldown <= 0) alertCooldown = postCalmAlertTime;
        }
        if (alertCooldown > 0) {
            alert = true;
            alertCooldown -= Time.deltaTime;
            if (alertCooldown <= 0) alert = false;
        }
    }

    void CheckDetect()
    {
        if (postAttackCalmCooldown > 0) return;

        float distToPlayer = Vector3.Distance(transform.position, PlayerManager.i.submarine.position);
        bool inRange = distToPlayer < hearRange;
        bool canHearPlayer = PlayerManager.i.GetNoisePecent() > soundThreshold;
        bool seenByPlayer = CheckLightDetect() && PlayerManager.i.headlightsOn;
        bool playerDetected = ( canHearPlayer || seenByPlayer ) && inRange;

        if (playerDetected) alertCooldown = InvestigationTime;
        if (shark && shark.scriptedBehavior) {
            timeAlert = 0;
        }
        else if (alert) timeAlert += Time.deltaTime;
        else timeAlert = 0;
        if (timeAlert >= alertToAgroTime) aggro = true;
    }

    bool CheckLightDetect()
    {
        int numGoodHits = 0;
        foreach (var point in lightCheckPoints) if (GoodHit(transform.TransformPoint(point), PlayerManager.i.steerCam.position, PlayerManager.i.steerCam.forward)) numGoodHits++;
        return numGoodHits > 0;
    }

    bool GoodHit(Vector3 point, Vector3 camPos, Vector3 camforward)
    {
        var dir = point - camPos;
        var maxDist = Vector3.Distance(point, camPos);
        var ray = new Ray(camPos, dir);
        bool hit = Physics.Raycast(ray, out var hitData, maxDist, raycastMask);
        bool inRange = Vector3.Distance(dir.normalized, camforward) <= Mathf.Sqrt(3) / 2;
        bool good = !hit || hitData.collider.GetComponentInParent<Enemy>() == this;

        //var debugColor = !inRange ? Color.magenta : (good ? Color.green : Color.red);
        //Debug.DrawLine(point, camPos, debugColor);
        //if (hit) print(hitData.collider.name);

        return good && inRange;
    }

    void TryScream()
    {
        screamCooldown -= Time.deltaTime;
        if (screamCooldown > 0) return;

        Scream.Play(transform);
        screamCooldown = Random.Range(screamWaitRange.x, screamWaitRange.y);
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var point in lightCheckPoints) Gizmos.DrawWireSphere(transform.TransformPoint(point), 0.5f);
        Gizmos.DrawWireSphere(transform.position, hearRange);
    }
}
