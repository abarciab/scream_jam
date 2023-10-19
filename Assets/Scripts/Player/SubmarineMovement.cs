using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

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

    public bool active;
    public float yawSpeed;
    public float pitchTime;
    public float propelForwardSpeed;
    public float propelBackwardSpeed;
    public float maxVelocityMagnitude;
    [Range(0, 90)]
    public float maxPitchAngle;
    public AnimationCurve yawGrowthSpeed;
    public AnimationCurve pitchCurve;
    public AnimationCurve dampenCurve;

    bool isPitching = false;
    float forwardAndBackInput;
    float yawInput;
    bool pitchDownInput;
    bool pitchUpInput;
    float normToMax;
    Rigidbody rb;

    #region Debug
    [Header("Debug")]

    [ReadOnly]
    [SerializeField]
    float velocityMagnitude;
    #endregion


    //Called by a unity event
    public void Activate()
    {
        active = true;
    }

    //Called by a unity event
    public void Deactivate()
    {
        active = false;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        active = false;
    }

    void Update()
    {
        if (!active)
        {
            return;
        }
        #region Input

        //Note: Temp Code
        forwardAndBackInput = Input.GetAxis("Vertical");
        yawInput = Input.GetAxis("Horizontal");

        pitchDownInput = Input.GetKey(KeyCode.LeftShift);
        pitchUpInput = Input.GetButton("Jump");
        //

        if (forwardAndBackInput != 0)
        {
            Propel(forwardAndBackInput);
        }

        //We can only yaw if we are moving
        if (yawInput != 0 && rb.velocity.magnitude > 0)
        {
            Yaw(yawInput);
        }

        //We can only pitch if we are moving
        if (isPitchingInput() && rb.velocity.magnitude > 0 && !isPitching)
        {
            StartCoroutine(Pitch(pitchUpInput ? -1 : 1));
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
        print("yawwwww");
        OnYawing?.Invoke(direction);
    }

    IEnumerator Pitch(float direction)
    {
        isPitching = true;
        float normTime = 0;
        Quaternion startingRot = transform.rotation;
        Quaternion endRot = transform.rotation;
        endRot.eulerAngles = new Vector3(maxPitchAngle * direction, startingRot.eulerAngles.y, startingRot.eulerAngles.z);

        while (normTime < 1.0f && isPitchingInput() && rb.velocity.magnitude > 0)
        {
            rb.MoveRotation(Quaternion.Lerp(startingRot, endRot, pitchCurve.Evaluate(normTime)));

            OnPitching?.Invoke(direction);
            normTime += Time.deltaTime / pitchTime;
            yield return null;
        }


        if (normTime < 1.0f)
        {
            float dampenDegree = 4f * normTime;
            float dampenTime = 1f * normTime;

            normTime = 0;

            startingRot = transform.rotation;
            transform.Rotate(new Vector3(dampenDegree * direction, 0, 0));
            endRot = transform.rotation;
            transform.rotation = startingRot;
            //go up 1 degree
            while (normTime < 1.0f)
            {
                OnPitching?.Invoke(direction);
                rb.MoveRotation(Quaternion.Lerp(startingRot, endRot, dampenCurve.Evaluate(normTime)));
                normTime += Time.deltaTime / dampenTime;
                yield return null;
            }

        }

        isPitching = false;
    }

    #region Utils
    bool isPitchingInput()
    {
        return pitchDownInput != pitchUpInput;
    }
    #endregion

}
