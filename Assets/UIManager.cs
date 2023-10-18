using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager current;
    private void Awake() { current = this; }
    [SerializeField] WaypointController wpController;
    [SerializeField] LogController logController;

    public void DisplayReadLogButtion(bool active)
    {
        wpController.hideNextRuin = active || logController.logActive;
        logController.DisplayReadLogButtion(active);
    }

    public void DisplayLog(string log)
    {
        logController.DisplayLog(log);
    }
}
