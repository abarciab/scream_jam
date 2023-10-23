using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaconEvent : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Player>()) Trigger();
    }

    void Trigger()
    {
        GetComponent<Collider>().enabled = false;

        FindObjectOfType<WaypointController>().ShowRuin();
    }

}
