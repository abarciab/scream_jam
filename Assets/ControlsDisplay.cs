using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsDisplay : MonoBehaviour
{
    [SerializeField] Animator steeringControls;
    bool exiting;

    private void Update()
    {

        if (PlayerManager.i.currentlySteering && PlayerManager.i.throttleEnabled) {
            exiting = false;
            steeringControls.gameObject.SetActive(true);
        }
        else if (!exiting) {
            steeringControls.SetTrigger("exit");
            exiting = true;
        }
    }
}
