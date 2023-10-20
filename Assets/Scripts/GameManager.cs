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

    private void Start()
    {
        if (startWithCheckpoint) {
            PlayerManager.i.submarine.transform.position = checkPoints[currentCheckpoint].position;
            PlayerManager.i.submarine.transform.rotation = checkPoints[currentCheckpoint].rotation;
        }
        LockCursor(true);
    }

    public void LockCursor(bool _lock)
    {
        Cursor.lockState = _lock ? CursorLockMode.Locked : CursorLockMode.Confined;
        Cursor.visible = !_lock;
    }
}
