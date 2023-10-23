using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideWhenTouchingPlayer : MonoBehaviour
{
    float showDist;
    bool hidden;

    private void Start()
    {
        showDist = 30;
    }

    public void Hide()
    {
        foreach (var r in GetComponentsInChildren<Renderer>()) r.enabled = false;
        hidden = true;
    }
    public void Show()
    {
        foreach (var r in GetComponentsInChildren<Renderer>()) r.enabled = true;
        hidden = false;
    }

    private void Update()
    {
        if (!hidden) return;
        float dist = Vector3.Distance(transform.position, PlayerManager.i.submarine.position);
        if (dist >= showDist) Show();
    }
}
