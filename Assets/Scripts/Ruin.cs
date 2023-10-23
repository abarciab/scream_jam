
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruin : MonoBehaviour
{
    [SerializeField] string displayName = "Ruin";
    [Min(1)] public int num;
    [SerializeField] float logReadRange = 4;
    [HideInInspector] public bool nextRuin;
    [SerializeField] List<LogCoordinator> logs = new List<LogCoordinator>();

    private void OnValidate()
    {
        gameObject.name = displayName + " (ruin " + num + ")";
    }

    public string GetName() { return displayName; }

    void Start()
    {
        EnvironmentManager.current.RegisterNewRuin(this);
        foreach (var l in logs) l.ruin = this;
    }

    public Vector3 GetPosition()
    {
        return logs[0].transform.position;
    }

    public void RemoveLog(LogCoordinator log)
    {
        logs.Remove(log);
        if (logs.Count == 0) Discover();
    }
    
    public void Discover()
    {
        EnvironmentManager.current.RemoveRuin(this);
        UIManager.current.DisplayReadLogButtion(false);

        nextRuin = false;
    }
}
