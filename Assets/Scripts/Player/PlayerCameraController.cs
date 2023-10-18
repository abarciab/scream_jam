using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using MyBox;

public class PlayerCameraController : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    float xRotation = 0f;

    [MyBox.ReadOnly]
    [SerializeField]
    bool locked;

    public void LockCamera()
    {
        locked = true;
    }

    void Start()
    {
        // Hide and lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (locked)
        {
            return;
        }
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Calculate the rotation for the camera vertically
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply the rotation to the camera
        transform.localRotation = Quaternion.Euler(xRotation, mouseX, 0f);
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
