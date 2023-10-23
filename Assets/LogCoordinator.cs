using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogCoordinator : MonoBehaviour
{
    [SerializeField] TextAsset logData;
    [HideInInspector] public Ruin ruin;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null) ReadLog();
    }

    void ReadLog()
    {
        UIManager.current.DisplayLog(logData.text);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        ruin.RemoveLog(this);
    }
}
