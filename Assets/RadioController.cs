using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class RadioController : MonoBehaviour
{
    [SerializeField] TextAsset introRadioText;
    [SerializeField] List<string> lines = new List<string>();
    int currentLine;
    [SerializeField] GameObject fToContinue;
    [SerializeField] Sound overSound;

    [SerializeField] Animator playerParent, otherParent;
    [SerializeField] TextMeshProUGUI playerText, otherText;

    [Space()]
    [SerializeField] GameObject fToPickup;
    [SerializeField] Sound ringSound;

    private void Start()
    {
        ringSound = Instantiate(ringSound);
        overSound = Instantiate(overSound);

        PlayerManager.i.throttleEnabled = false;
        ProcessText();
    }


    [ButtonMethod]
    void ProcessText()
    {
        lines = introRadioText.text.Split("\n").ToList();
    }

    private void Update()
    {
        if (currentLine == 0 && !PlayerManager.i.engineOn) return;
        else if (currentLine == 0 && PlayerManager.i.engineOn) {
            ringSound.Play(restart: false);
            fToPickup.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.P)) EndConversation();

        if (Input.GetKeyDown(KeyCode.F)) {
            DisplayNextLine();
        }
    }

    void DisplayNextLine()
    {
        ringSound.Stop();
        fToPickup.SetActive(false);
        fToContinue.SetActive(false);
        fToContinue.SetActive(true);

        if (currentLine >= lines.Count) {
            playerParent.SetBool("active", false);
            otherParent.SetBool("active", false);
            EndConversation();
            return;
        }

        if (lines[currentLine].Length < 8) currentLine += 1;
        else if (currentLine > 0) overSound.Play();
        bool playerLine = currentLine % 2 == 0;

        if (playerLine) {
            ShowText(playerParent, playerText, otherParent, lines[currentLine]);
        }
        else {
            ShowText(otherParent, otherText, playerParent, lines[currentLine]);
        }
        currentLine += 1;
    }

    void ShowText(Animator activeParent, TextMeshProUGUI activeText, Animator inactiveParent, string line)
    {
        activeParent.gameObject.SetActive(true);
        activeParent.SetBool("active", true);
        inactiveParent.SetBool("active", false);
        activeText.text = line;
    }

    void EndConversation()
    {
        PlayerManager.i.throttleEnabled = true;
        gameObject.SetActive(false);
    }
}
