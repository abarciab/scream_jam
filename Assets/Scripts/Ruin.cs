
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruin : MonoBehaviour
{
    [SerializeField] string displayName = "Ruin";
    [Min(1)] public int num;
    [SerializeField] float logReadRange = 4;
    [HideInInspector] public bool nextRuin;
    [SerializeField] TextAsset logText;

    private void OnValidate()
    {
        gameObject.name = displayName + " (ruin " + num + ")";
    }

    void Start()
    {
        EnvironmentManager.current.RegisterNewRuin(this);
    }

    private void Update()
    {
        if (!nextRuin) return;

        var dist = Vector3.Distance(transform.position, Camera.main.transform.position);
        bool inRange = dist < logReadRange;
        UIManager.current.DisplayReadLogButtion(inRange);
        if (inRange && Input.GetKeyDown(KeyCode.F)) ReadLog(); 
    }

    void ReadLog()
    {
        UIManager.current.DisplayLog(logText.text);
        Discover();
    }

    public void Discover()
    {
        EnvironmentManager.current.RemoveRuin(this);
        UIManager.current.DisplayReadLogButtion(false);

        nextRuin = false;
    }
}
