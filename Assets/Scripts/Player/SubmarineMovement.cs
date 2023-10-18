using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEditor.Experimental.GraphView;


[RequireComponent(typeof(Rigidbody))]
public class SubmarineMovement : MonoBehaviour
{

    #region Events
    // Called on each frame when the sub is propelling
    public Action<float> OnPropelling;
    // Called on each frame when the sub is yawing
    public Action<float> OnYawing;
    // Called on each frame when the sub is pitching
    public Action<float> OnPitching;
    #endregion


    public float yawSpeed;
    public float pitchSpeed;
    public float propelForwardSpeed;
    public float propelBackwardSpeed;
    public float maxVelocityMagnitude;
    [Range(0, 90)]
    public float maxPitchAngle;
    public AnimationCurve yawGrowthSpeed;
    public AnimationCurve pitchGrowthSpeed;

    float normToMax;
    Rigidbody rb;

    #region Debug
    [Header("Debug")]

    [ReadOnly]
    [SerializeField]
    float velocityMagnitude;
    #endregion


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        #region Input

        //Note: Temp Code
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        bool spaceInput = Input.GetButton("Jump");
        bool crlInput = Input.GetKey(KeyCode.LeftControl);
        //

        if (verticalInput != 0)
        {
            Propel(verticalInput);
        }

        //We can only rotate if we are moving
        if (horizontalInput != 0 && rb.velocity.magnitude > 0)
        {
            Yaw(horizontalInput);
        }

        if (spaceInput || crlInput)
        {
            Pitch(crlInput ? 1 : -1);
        }
        #endregion


        #region Debug
        velocityMagnitude = rb.velocity.magnitude;
        #endregion
    }

    private void FixedUpdate()
    {
        normToMax = rb.velocity.magnitude / maxVelocityMagnitude;

        // Cap velocity if it exceeds the maximum velocity magnitude
        if (rb.velocity.magnitude > maxVelocityMagnitude)
        {
            rb.velocity = rb.velocity.normalized * maxVelocityMagnitude;
        }

        // Cap pitch angle 
        float deltaAngle = Mathf.DeltaAngle(transform.rotation.eulerAngles.x, 0);
        if (Mathf.Abs(deltaAngle) > maxPitchAngle)
        {
            Vector3 currentRotation = transform.rotation.eulerAngles;
            float cappedRotationX = Mathf.Sign(deltaAngle) * maxPitchAngle;
            transform.rotation = Quaternion.Euler(-cappedRotationX, currentRotation.y, currentRotation.z);
        }


    }

    void Propel(float direction)
    {
        Vector3 vDirection = transform.forward * direction;
        float speed = direction > 0 ? propelForwardSpeed : propelBackwardSpeed;
        rb.AddForce(speed * vDirection, ForceMode.Acceleration);
        OnPropelling?.Invoke(direction);
    }

    void Yaw(float direction)
    {
        Quaternion deltaRotation = Quaternion.Euler(0f, direction *
                                                        (yawGrowthSpeed.Evaluate(normToMax) * yawSpeed)
                                                        * Time.deltaTime, 0f);
        rb.MoveRotation(rb.rotation * deltaRotation);
        OnYawing?.Invoke(direction);
    }

    void Pitch(float direction)
    {
        Quaternion deltaRotation = Quaternion.Euler(direction *
                                                        (pitchGrowthSpeed.Evaluate(normToMax) * pitchSpeed)
                                                        * Time.deltaTime, 0f, 0f);

        float deltaAngle = Mathf.DeltaAngle(deltaRotation.eulerAngles.x, 0);
        if (Mathf.Abs(deltaAngle) > maxPitchAngle)
        {
            return;
        }

        rb.MoveRotation(rb.rotation * deltaRotation);
        OnPitching?.Invoke(direction);
    }

}