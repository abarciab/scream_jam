using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogController : MonoBehaviour
{
    [SerializeField] GameObject readLogButton, logParent;
    [SerializeField] TextMeshProUGUI logMainText;
    [SerializeField] ScrollRect scrollRect;
    public bool logActive { get { return logParent.activeInHierarchy; } }
    public void DisplayReadLogButtion(bool active)
    {
        readLogButton.SetActive(active);
    }

    public void DisplayLog(string text)
    {
        scrollRect.verticalNormalizedPosition = 1;
        logMainText.text = text;
        logParent.SetActive(true);
        GameManager.i.LockCursor(false);
        PlayerManager.i.moveScript.StopThrottle();
    }

    public void NextRuin()
    {
        HideLog();
    }

    void HideLog()
    {
        logParent.SetActive(false);
        GameManager.i.LockCursor(true);
    }
}
