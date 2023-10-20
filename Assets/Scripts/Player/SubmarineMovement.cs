using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEditor.Experimental.GraphView;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using System.Data;

[RequireComponent(typeof(Rigidbody))]
public class SubmarineMovement : MonoBehaviour
{
    [SerializeField] bool active;
    [SerializeField] float turnSpeed, moveSpeed, turnSmoothness = 0.1f, accelerationSmoothness = 0.1f, propelForwardSpeed, propelBackwardSpeed;
    Quaternion turnDelta;

    [Header("Throttle")]
    [Range(-1, 1)] public float currentThrottle;
    [SerializeField] float throttleChangeSpeed, minThrottleSpeed, slowDownMod;

    [Header("Sounds")]
    [SerializeField] Sound propSound;

    [Header("spinning")]
    [SerializeField] float spinTime;
    [SerializeField] AnimationCurve spinXCurve, spinYCurve, spinZCurve;
    [SerializeField] float spinSmoothTime = 2, spinSmoothness = 0.05f;
    bool spinning;

    Rigidbody rb;

    public void Activate() { active = true; }
    public void Deactivate() { active = false; }

    public float totalThrottle { get { return Mathf.Abs(currentThrottle) - minThrottleSpeed; } }

    [ButtonMethod]
    public void Spin()
    {
        StartCoroutine(AnimateSpin());
    }

    IEnumerator AnimateSpin()
    {
        var startRot = transform.localEulerAngles;
        var startQuat = transform.rotation;
        spinning = true;
        var deltaRot = Vector3.zero;
        float timePassed = 0;
        while (timePassed < spinTime) {
            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();

            float progress = timePassed / spinTime;
            var euler = Vector3.one * 360;
            euler.x *= spinXCurve.Evaluate(progress);
            euler.y *= spinYCurve.Evaluate(progress);
            euler.z *= spinZCurve.Evaluate(progress);
            deltaRot = Vector3.Lerp(deltaRot, euler, spinSmoothness);

            transform.localEulerAngles = deltaRot + startRot;
        }

        timePassed = 0;
        while (timePassed < spinSmoothTime) {
            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            transform.localRotation = Quaternion.Lerp(transform.rotation, startQuat, spinSmoothness);
        }

        transform.rotation = startQuat;
        spinning = false;
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
    }

    public void StopThrottle()
    {
        currentThrottle = 0;
    }

    void Update()
    {
        propSound.PercentVolume(Mathf.Abs(currentThrottle), 0.1f);
        if (!PlayerManager.i.engineOn || spinning) {
            currentThrottle = 0;
            return;
        }

        rb.angularVelocity = Vector3.zero;
        if (Input.GetKey(KeyCode.LeftShift) && currentThrottle > -1) currentThrottle -= throttleChangeSpeed * slowDownMod * Time.deltaTime;
        if (Input.GetKey(KeyCode.Space) && currentThrottle < 1) currentThrottle += throttleChangeSpeed * Time.deltaTime;
        Propel();

        Vector3 inputDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) inputDir += new Vector3(-1, 0, 0);
        if (Input.GetKey(KeyCode.S)) inputDir += new Vector3(1, 0, 0);
        if (Input.GetKey(KeyCode.A)) inputDir += new Vector3(0, -1, 0);
        if (Input.GetKey(KeyCode.D)) inputDir += new Vector3(0, 1, 0);
        if (Input.GetKey(KeyCode.E)) inputDir += new Vector3(0, 0, -1);
        if (Input.GetKey(KeyCode.Q)) inputDir += new Vector3(0, 0, 1);
        var deltaEuler = Time.deltaTime * turnSpeed * inputDir;
        var targetDelta = Quaternion.Euler(deltaEuler);
        turnDelta = Quaternion.Lerp(turnDelta, targetDelta, turnSmoothness);
        transform.rotation *= turnDelta;
    }

    void Propel()
    {
        if (Mathf.Abs(currentThrottle) < minThrottleSpeed) return;
        Vector3 targetSpeed = currentThrottle * moveSpeed * transform.forward;
        rb.velocity = Vector3.Lerp(rb.velocity, targetSpeed, accelerationSmoothness);
    }
}
