using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermediateWaypoint : MonoBehaviour
{
    public int ruinId;

    private void Start()
    {
        EnvironmentManager.current.intermediateWaypoints.Add(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null) {
            EnvironmentManager.current.intermediateWaypoints.Remove(this);
            Destroy(gameObject);
        }
    }
}
