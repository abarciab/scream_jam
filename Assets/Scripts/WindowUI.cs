using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI depthText;
    [SerializeField] Transform wheelParent;
    [SerializeField] GameObject depthParent, compassParent, throttleParent, restartParent, headLightsOff;
    [SerializeField] bool showCompass;
    [SerializeField] Slider healthSlider, throttleSlider, restartSlider;
    [SerializeField] GameObject poewrOffText;
    [SerializeField] SubmarineMovement moveScript;
    [SerializeField] Transform upArrow;

    [Header("noiseIndicator")]
    [SerializeField] Image noiseImage;
    [SerializeField] Sprite noise0, noise1, noise2, noise3;
    [SerializeField, Range(0, 1)] float noise1Threshold, noise2Threshold, noise3Threshold;
    [SerializeField] TextMeshProUGUI noiseText;
    [SerializeField] string noise0string, noise1string, noise2string, noise3string;

    [Header("Oxygen")]
    [SerializeField] GameObject oxygenParent;
    [SerializeField] TextMeshProUGUI oxygenText;

    private void Update()
    {
        bool engineOn = PlayerManager.i.engineOn;
        poewrOffText.SetActive(!engineOn);
        depthParent.SetActive(engineOn);
        throttleParent.SetActive(engineOn);
        compassParent.SetActive(engineOn && showCompass);
        healthSlider.value = PlayerManager.i.player.healthPercent;
        headLightsOff.SetActive(!PlayerManager.i.headlightsOn);

        float waitTimeLeft = PlayerManager.i.player.getWaitTimeLeftPercent();
        restartParent.SetActive(engineOn && waitTimeLeft > 0 && !PlayerManager.i.moveScript.enabled);
        restartSlider.value = waitTimeLeft;

        float depth = GameManager.i.SeaLevel - transform.position.y;
        depthText.text = (Mathf.Round(depth * 10) / 10) + " m";

        wheelParent.transform.localEulerAngles = new Vector3(0, 0, PlayerManager.i.submarine.transform.localEulerAngles.y);

        throttleSlider.value = (moveScript.currentThrottle + 1) / 2;

        upArrow.rotation = Quaternion.identity;

        UpdateNoiseGraphic();
        ShowOxygen();
    }

    void ShowOxygen()
    {
        oxygenParent.SetActive(PlayerManager.i.lowOxygen);
        oxygenText.text = "oxygen: " + PlayerManager.i.oxygenPercent + "%";
    }

    void UpdateNoiseGraphic()
    {
        float noise = PlayerManager.i.GetNoisePecent();
        if (noise > noise3Threshold) DisplayNoise(3);
        else if (noise > noise2Threshold) DisplayNoise(2);
        else if (noise > noise1Threshold) DisplayNoise(1);
        else DisplayNoise(0);
    }

    void DisplayNoise(int num)
    {
        switch (num) {
            case 0:
                noiseImage.sprite = noise0;
                noiseText.text = noise0string;
                break;
            case 1:
                noiseImage.sprite = noise1;
                noiseText.text = noise1string;
                break;
            case 2:
                noiseImage.sprite = noise2;
                noiseText.text = noise2string;
                break;
            case 3:
                noiseImage.sprite = noise3;
                noiseText.text = noise3string;
                break;
        }
    }
}
