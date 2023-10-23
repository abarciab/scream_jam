using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsDisplay : MonoBehaviour
{
    [SerializeField] Animator steeringControls, tab;
    bool hidingSteeringControls, hidingTab;

    private void Update()
    {
        bool showTab = PlayerManager.i.currentlySteering || PlayerManager.i.lookingAtRadar;

        if (showTab) {
            tab.gameObject.SetActive(true);
            hidingTab = false;
        }
        else if (!hidingTab) {
            hidingTab = true;
            tab.SetTrigger("exit");
        }


        if (PlayerManager.i.currentlySteering && PlayerManager.i.throttleEnabled) {
            hidingSteeringControls = false;
            steeringControls.gameObject.SetActive(true);
        }
        else if (!hidingSteeringControls) {
            steeringControls.SetTrigger("exit");
            hidingSteeringControls = true;
        }
    }
}
