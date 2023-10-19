using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WindowUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI depthText;
    [SerializeField] Transform wheelParent;
    [SerializeField] GameObject depthParent, compassParent;

    private void Update()
    {
        bool engineOn = PlayerManager.Instance.submarine.GetComponent<SubmarineMovement>().engineOn;
        depthParent.SetActive(engineOn);
        compassParent.SetActive(engineOn);

        float depth = GameManager.i.SeaLevel - transform.position.y;
        depthText.text = (Mathf.Round(depth * 10) / 10) + " m";

        wheelParent.transform.localEulerAngles = new Vector3(0, 0, PlayerManager.Instance.submarine.transform.localEulerAngles.y);
    }
}
