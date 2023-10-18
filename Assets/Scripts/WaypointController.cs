using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointController : MonoBehaviour
{
    [SerializeField] float alwaysDisplayRange;
    [SerializeField] bool displayWaypoints;
    [SerializeField] Transform nextRuinWaypoint;
    [SerializeField] Vector2 wayPointAnchoredPositionLimits;

    Vector3 nextRuin;
    EnvironmentManager eMan;
    Transform player;
    [HideInInspector] public bool hideNextRuin;

    public void SetWaypointMode(bool active)
    {
        displayWaypoints = active;
    }

    private void Start()
    {
        eMan = EnvironmentManager.current;
        player = Camera.main.transform;
    }

    private void Update()
    {
        nextRuin = eMan.GetNextRuinPos();

        var dist = Vector3.Distance(player.position, nextRuin);
        if (dist < alwaysDisplayRange) DisplayNextRuin(true);

        HideWaypoints();
        if (displayWaypoints) DisplayWaypoints();
    }

    void HideWaypoints()
    {
        nextRuinWaypoint.gameObject.SetActive(false);
    }

    void DisplayWaypoints()
    {
        DisplayNextRuin();
    }

    void DisplayNextRuin(bool overrideFacing = false)
    {
        if (hideNextRuin || !EnvironmentManager.current.nextRuinAvaliable) return;

        nextRuinWaypoint.gameObject.SetActive(true);
        var rt = nextRuinWaypoint.GetComponent<RectTransform>();
        var oldPos = rt.anchoredPosition;

        nextRuinWaypoint.transform.position = Camera.main.WorldToScreenPoint(nextRuin);
        bool facingNextRuin = GetAngleToObject(nextRuin) > 0;

        if (!facingNextRuin) DisplayOffScreenWaypoint(rt, oldPos);
        else {
            ClampWaypointToLimits(rt);
        }
    }

    void ClampWaypointToLimits(RectTransform wp)
    {
        var pos = wp.anchoredPosition;
        pos.x = Mathf.Clamp(pos.x, -wayPointAnchoredPositionLimits.x, wayPointAnchoredPositionLimits.x);
        pos.y = Mathf.Clamp(pos.y, -wayPointAnchoredPositionLimits.y, wayPointAnchoredPositionLimits.y);
        wp.anchoredPosition = pos;
    }

    void DisplayOffScreenWaypoint(RectTransform wp, Vector2 oldPos)
    {
        var pos = wp.anchoredPosition;

        float deltaX = Mathf.Abs(pos.x - wayPointAnchoredPositionLimits.x);
        float deltaY = Mathf.Abs(pos.y - wayPointAnchoredPositionLimits.y);

        if (deltaY > deltaX) {
            if (pos.x > 0) pos.x = -wayPointAnchoredPositionLimits.x;
            else pos.x = wayPointAnchoredPositionLimits.x;
            pos.y *= -1;
            pos.y = Mathf.Clamp(pos.y, -wayPointAnchoredPositionLimits.y, wayPointAnchoredPositionLimits.y);
        }
        else {
            if (pos.y > 0) pos.y = -wayPointAnchoredPositionLimits.y;
            else pos.y = wayPointAnchoredPositionLimits.y;
            pos.x *= -1;
            pos.x = Mathf.Clamp(pos.x, -wayPointAnchoredPositionLimits.x, wayPointAnchoredPositionLimits.x);
        }
        

        wp.anchoredPosition = Vector2.Lerp(oldPos, pos, 0.1f);
    }


    float GetAngleToObject(Vector3 pos)
    {
        var camForward = Camera.main.transform.forward;
        var objDir = pos - Camera.main.transform.position;
        camForward.y = 0;
        objDir.y = 0;
        float angle = Vector3.Dot(camForward, objDir);

        return angle / 180;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(Camera.main.transform.position, alwaysDisplayRange);
    }
}
