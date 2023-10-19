using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEditor.Experimental.GraphView;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;

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
    public float pitchSpeed;
    public float propelForwardSpeed;
    public float propelBackwardSpeed;
    public float maxVelocityMagnitude;
    [Range(0, 90)]
    public float maxPitchAngle;
    public AnimationCurve yawGrowthSpeed;
    public AnimationCurve pitchGrowthSpeed;

    float normToMax;
    float timeSpentHoldingYawControls, timeSpentHoldingPitchControls;
    [SerializeField] float timeToFullPitchAndYaw = 1;

    [Header("Sounds")]
    [SerializeField] Sound engineSound;
    [SerializeField] Sound propSound;
    [SerializeField] float engineToggleTime = 2;
    public bool engineOn;
   
    Rigidbody rb;

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

    private void Start()
    {
        propSound = Instantiate(propSound);
        propSound.PlaySilent();
        engineSound = Instantiate(engineSound);
        engineSound.PlaySilent();
        if (engineOn) TurnOnEngine();
    }

    public void TurnOffEngine()
    {
        StartCoroutine(SetEngine(false));
    }

    public void TurnOnEngine()
    {
        StartCoroutine(SetEngine(true));
    }

    IEnumerator SetEngine(bool active)
    {
        float target = active ? 1 : 0;
        float start = active ? 0 : 1;
        float timePassed = 0;
        while (timePassed < engineToggleTime) {
            engineSound.PercentVolume(Mathf.Lerp(start, target, timePassed / engineToggleTime));
            yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;
        }
        engineSound.PercentVolume(target);
        engineOn = active;
    }

    void Update()
    {
        if (!active || !engineOn)
        {
            return;
        }

        Propel(Input.GetAxis("Vertical"));
        Yaw(Input.GetAxis("Horizontal"));

        if (Input.GetButton("Jump")) Pitch(-1);
        else if (Input.GetKey(KeyCode.LeftShift)) Pitch(1);
        else timeSpentHoldingPitchControls = 0;
        
    }

    private void FixedUpdate()
    {
        normToMax = rb.velocity.magnitude / maxVelocityMagnitude;
        propSound.PercentVolume(normToMax);

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
        if (direction == 0) {
            timeSpentHoldingYawControls = 0;
            return;
        }

        timeSpentHoldingYawControls += Time.deltaTime;
        float progress = timeSpentHoldingYawControls / timeToFullPitchAndYaw;
        progress = Mathf.Clamp01(progress);

        float yRot = direction * (yawGrowthSpeed.Evaluate(progress) * yawSpeed) * Time.deltaTime;
        Quaternion deltaRotation = Quaternion.Euler(0f, yRot, 0f);
        rb.MoveRotation(rb.rotation * deltaRotation);
        OnYawing?.Invoke(direction);
    }

    void Pitch(float direction)
    {
        timeSpentHoldingPitchControls += Time.deltaTime;
        float progress = timeSpentHoldingPitchControls / timeToFullPitchAndYaw;
        progress = Mathf.Clamp01(progress);

        float xRot = direction * (pitchGrowthSpeed.Evaluate(progress) * pitchSpeed) * Time.deltaTime;
        Quaternion deltaRotation = Quaternion.Euler(xRot, 0f, 0f);
        float deltaAngle = Mathf.DeltaAngle(deltaRotation.eulerAngles.x, 0);
        
        if (Mathf.Abs(deltaAngle) > maxPitchAngle) return;
        rb.MoveRotation(rb.rotation * deltaRotation);
        OnPitching?.Invoke(direction);
    }

}
