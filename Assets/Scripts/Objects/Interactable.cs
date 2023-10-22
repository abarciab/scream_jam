using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;
using Cinemachine;
using UnityEngine.UIElements;

public class Interactable : MonoBehaviour
{
    [Tooltip("Lock the camera and change to vcam")]
    public bool doLockState;
    [SerializeField] bool steeringCam;


    [ConditionalField(nameof(doLockState))]
    public CinemachineVirtualCamera vCam;

    public UnityEvent OnInteract;
    public UnityEvent OnHover;
    public UnityEvent OnHoverExit;

    [ConditionalField(nameof(doLockState))]
    public UnityEvent OnLockStateEnter;
    [ConditionalField(nameof(doLockState))]
    public UnityEvent OnLockStateExit;

    [Header("Color changeOnHover")]
    [SerializeField] bool changeColorOnHover = true;
    [SerializeField] bool dontHoverWhenActive;
    [SerializeField] List<Renderer> renderers = new List<Renderer>();
    [SerializeField] Material normalMat, HoverMat;
    [SerializeField] GameObject hoverText;
    bool activated;

    [Header("Sounds")]
    [SerializeField] Sound errorSound;

    public bool inLockState { get { return _inLockState; } }

    [ReadOnly]
    bool _inLockState;

    private void Start()
    {
        if (errorSound && !errorSound.instantialized) errorSound = Instantiate(errorSound);
        OnMouseExit();
    }

    private void OnMouseExit()
    {
        OnHover.Invoke();
        if (!changeColorOnHover) return;
        ChangeColor(normalMat);
        hoverText.SetActive(false);
    }

    private void OnMouseEnter()
    {
        OnHover.Invoke();
        if (!changeColorOnHover || dontHoverWhenActive && activated) return;
        ChangeColor(HoverMat);
        hoverText.SetActive(true);
    }

    void ChangeColor(Material mat)
    {
        foreach (var r in renderers) r.material = mat;
    }

    public void Interact()
    {
        if (!enabled) {
            if (errorSound) {
                if (!errorSound.instantialized) errorSound = Instantiate(errorSound);
                errorSound.Play(transform);
            }
            return;
        }

        activated = !activated;
        if (steeringCam) PlayerManager.i.currentlySteering = activated;

        if (activated && dontHoverWhenActive) {
            ChangeColor(normalMat);
            hoverText.SetActive(false);
        }
        if (inLockState) return;

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
            if (steeringCam) PlayerManager.i.currentlySteering = false;
            ExitLockState();
        }
    }

    void EnterLockState()
    {
        _inLockState = true;
        PlayerManager.i.playerCameraController.playerVCam.Priority = 0;
        PlayerManager.i.playerCameraController.LockCamera();
        vCam.Priority = 100;
        OnLockStateEnter.Invoke();
        activated = true;
    }

    void ExitLockState()
    {
        _inLockState = false;
        PlayerManager.i.playerCameraController.playerVCam.Priority = 100;
        PlayerManager.i.playerCameraController.UnlockCamera();
        vCam.Priority = 0;
        OnLockStateExit.Invoke();
        activated = false;
    }
}
