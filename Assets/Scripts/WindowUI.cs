using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI depthText;
    [SerializeField] Transform wheelParent;
    [SerializeField] GameObject depthParent, compassParent, throttleParent, restartParent;
    [SerializeField] bool showCompass;
    [SerializeField] Slider healthSlider, throttleSlider, restartSlider;
    [SerializeField] GameObject poewrOffText;
    [SerializeField] SubmarineMovement moveScript;
    [SerializeField] Transform upArrow;

    private void Update()
    {
        bool engineOn = PlayerManager.i.engineOn;
        poewrOffText.SetActive(!engineOn);
        depthParent.SetActive(engineOn);
        throttleParent.SetActive(engineOn);
        compassParent.SetActive(engineOn && showCompass);
        healthSlider.value = PlayerManager.i.player.healthPercent;

        float waitTimeLeft = PlayerManager.i.player.getWaitTimeLeftPercent();
        restartParent.SetActive(engineOn && waitTimeLeft > 0 && !PlayerManager.i.moveScript.enabled);
        restartSlider.value = waitTimeLeft;

        float depth = GameManager.i.SeaLevel - transform.position.y;
        depthText.text = (Mathf.Round(depth * 10) / 10) + " m";

        wheelParent.transform.localEulerAngles = new Vector3(0, 0, PlayerManager.i.submarine.transform.localEulerAngles.y);

        throttleSlider.value = (moveScript.currentThrottle + 1) / 2;

        upArrow.rotation = Quaternion.identity; 
    }
}
