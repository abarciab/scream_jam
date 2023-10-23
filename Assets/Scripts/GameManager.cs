using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager i;
    private void Awake() { i = this; }

    public int SeaLevel;

    [Header("Checkpoints")]
    [SerializeField] bool startWithCheckpoint;
    [SerializeField] List<Transform> checkPoints;
    [SerializeField] int currentCheckpoint;

    [Space()]
    [SerializeField] Vector2 startingFailRate;

    [Header("Pause")]
    [SerializeField] GameObject pauseMenu;

    private void Start()
    {
        if (startWithCheckpoint) {
            if (currentCheckpoint == 0) FindObjectOfType<RadioController>(true).gameObject.SetActive(true);
            PlayerManager.i.UpdateEngineFailRange(startingFailRate);
            JumpToCheckpoint();
        }
        LockCursor(true);
    }

    void JumpToCheckpoint()
    {
        PlayerManager.i.submarine.transform.position = checkPoints[currentCheckpoint].position;
        PlayerManager.i.submarine.transform.rotation = checkPoints[currentCheckpoint].rotation;
        print("loaded checkpoint " + currentCheckpoint);
    }

    public void LockCursor(bool _lock)
    {
        Cursor.lockState = _lock ? CursorLockMode.Locked : CursorLockMode.Confined;
        Cursor.visible = !_lock;
    }

    [ButtonMethod]
    public void PauseGame()
    {
        Time.timeScale = 0;
        AudioManager.instance.Pause();
        pauseMenu.SetActive(true);
        LockCursor(false);
    }

    [ButtonMethod]
    public void Resume()
    {
        Time.timeScale = 1;
        AudioManager.instance.Resume();
        LockCursor(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            PauseGame();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (currentCheckpoint < checkPoints.Count) currentCheckpoint += 1;
            JumpToCheckpoint();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if (currentCheckpoint > 0) currentCheckpoint -= 1;
            JumpToCheckpoint();
        }
    }
}
