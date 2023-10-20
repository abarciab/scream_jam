using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Sound Scream;
    [SerializeField] Vector2 screamWaitRange = new Vector2(5, 20);
    float screamCooldown;
    public bool aggro, alert;
    bool lightsOnMe;
    [SerializeField] LayerMask raycastMask;

    [SerializeField] List<Vector3> lightCheckPoints = new List<Vector3>();

    private void Start()
    {
        Scream = Instantiate(Scream);
        screamCooldown = Random.Range(screamWaitRange.x, screamWaitRange.y);
    }

    private void Update()
    {
        TryScream();
        bool canHearPlayer = PlayerManager.i.moveScript.totalThrottle > 0;

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

        print("hit: " + hit + (hit ? "hit.name: " + hitData.collider.name : ""));

        bool inRange = Vector3.Distance(dir.normalized, camforward) <= Mathf.Sqrt(3);

        bool good = !hit || hitData.collider.GetComponentInParent<Enemy>() == this;
        Debug.DrawLine(point, camPos, inRange ? (good ? Color.green : Color.red) : Color.magenta);
        return good;
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
    }
}
