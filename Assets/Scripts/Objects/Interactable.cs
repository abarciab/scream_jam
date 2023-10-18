using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;
using Cinemachine;


public class Interactable : MonoBehaviour
{
    [Tooltip("Lock the camera and change to vcam")]
    public bool doLockState;


    [ConditionalField(nameof(doLockState))]
    public CinemachineVirtualCamera vCam;

    public UnityEvent OnInteract;

    [ConditionalField(nameof(doLockState))]
    public UnityEvent OnLockStateEnter;
    [ConditionalField(nameof(doLockState))]
    public UnityEvent OnLockStateExit;


    public bool inLockState { get { return _inLockState; } }

    [ReadOnly]
    bool _inLockState;

    public void Interact()
    {
        if (inLockState)
        {
            return;
        }

        if (doLockState)
        {
            EnterLockState();
        }
        OnInteract.Invoke();
    }

    void Update()
    {
        if (!inLockState)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ExitLockState();
        }
    }

    void EnterLockState()
    {
        _inLockState = true;
        PlayerManager.Instance.playerCameraController.playerVCam.Priority = 0;
        PlayerManager.Instance.playerCameraController.LockCamera();
        vCam.Priority = 100;
        OnLockStateEnter.Invoke();
    }

    void ExitLockState()
    {
        _inLockState = false;
        PlayerManager.Instance.playerCameraController.playerVCam.Priority = 100;
        PlayerManager.Instance.playerCameraController.UnlockCamera();
        vCam.Priority = 0;
        OnLockStateExit.Invoke();

    }
}
