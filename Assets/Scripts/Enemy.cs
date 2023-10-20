using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    [SerializeField] Sound Scream;
    [SerializeField] Vector2 screamWaitRange = new Vector2(5, 20);
    float screamCooldown;
    [SerializeField] float hearRange = 20;
    public bool aggro, alert;
    bool lightsOnMe;
    [SerializeField] LayerMask raycastMask;

    [SerializeField] List<Vector3> lightCheckPoints = new List<Vector3>();

    [Header("Behavior")]
    [SerializeField] float attackCalmTime = 3;
    [SerializeField] float postCalmAlertTime = 5;
    float calmCooldown, alertCooldown;

    private void Start()
    {
        Scream = Instantiate(Scream);
        screamCooldown = Random.Range(screamWaitRange.x, screamWaitRange.y);
    }

    public void EnterCalm()
    {
        calmCooldown = attackCalmTime;
        aggro = false;
        alert = true;
    }

    private void Update()
    {
        TryScream();

        
        if (alertCooldown > 0) {
            alertCooldown -= Time.deltaTime;
            if (alertCooldown <= 0) alert = false;
        }
        if (calmCooldown > 0) {
            calmCooldown -= Time.deltaTime;
            transform.LookAt(PlayerManager.i.submarine.position);
            if (calmCooldown <= 0) alertCooldown = postCalmAlertTime;
            return;
        }

        float distToPlayer = Vector3.Distance(transform.position, PlayerManager.i.submarine.position);
        bool canHearPlayer = PlayerManager.i.moveScript.totalThrottle > 0 && distToPlayer < hearRange;
        bool seenByPlayer = CheckLightDetect();
        lightsOnMe = PlayerManager.i.headlightsOn && seenByPlayer;
        if (alert && (canHearPlayer || lightsOnMe)) aggro = true;
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
        bool inRange = Vector3.Distance(dir.normalized, camforward) <= Mathf.Sqrt(3);
        bool good = !hit || hitData.collider.GetComponentInParent<Enemy>() == this;
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
