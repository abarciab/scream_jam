using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideWhenTouchingPlayer : MonoBehaviour
{
    public void Hide()
    {
        foreach (var r in GetComponentsInChildren<Renderer>()) r.enabled = false;
    }
    public void Show()
    {
        foreach (var r in GetComponentsInChildren<Renderer>()) r.enabled = true;
    }
}
