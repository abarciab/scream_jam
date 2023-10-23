using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using MyBox;
using Cinemachine;

public class PlayerCameraController : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public CinemachineVirtualCamera playerVCam;
    public bool locked { get { return _locked; } }

    [SerializeField] Vector2 freeLookLimits;
    [SerializeField] bool limitCam;
    bool _locked;
    [SerializeField] float delay = 1.5f;

    public void LockCamera()
    {
        _locked = true;
    }

    public void UnlockCamera()
    {
        _locked = false;
    }

    void LateUpdate()
    {
        if (_locked) return;
        if (delay > 0) {
            delay -= Time.deltaTime;
            return;
        }

        float deltaY = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float deltaX = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        var deltaEuler = new Vector3(-deltaX, deltaY, 0f);
        var targetDelta = Quaternion.Euler(deltaEuler);
        transform.rotation *= targetDelta;

        if (!limitCam) return;
        var eulers = transform.localEulerAngles;
        if (eulers.y > 180) eulers.y -= 360;
        if (eulers.x > 180) eulers.x -= 360;
        eulers.x = Mathf.Clamp(eulers.x, -freeLookLimits.x, freeLookLimits.x);
        eulers.y = Mathf.Clamp(eulers.y, -freeLookLimits.y, freeLookLimits.y);
        transform.localEulerAngles = eulers;
    }
}
