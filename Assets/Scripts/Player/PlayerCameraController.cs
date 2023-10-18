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
    float xRotation = 0f;

    [MyBox.ReadOnly]
    [SerializeField]
    bool _locked;

    public void LockCamera()
    {
        _locked = true;
    }

    public void UnlockCamera()
    {
        _locked = false;
    }

    void Start()
    {
        // Hide and lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (_locked)
        {
            return;
        }
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, mouseX, 0f);
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
