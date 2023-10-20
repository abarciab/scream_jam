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
    [SerializeField] bool active;
    [SerializeField] float turnSpeed, moveSpeed, turnSmoothness = 0.1f, accelerationSmoothness = 0.1f, propelForwardSpeed, propelBackwardSpeed;
    Quaternion turnDelta;

    [Header("Throttle")]
    [Range(-1, 1)] public float currentThrottle;
    [SerializeField] float throttleChangeSpeed, minThrottleSpeed, slowDownMod;

    [Header("Sounds")]
    [SerializeField] Sound propSound;

    [Header("test")]
    [SerializeField] Vector3 testVector;
    [SerializeField] float testTurnSpeed;
   
    Rigidbody rb;

    public void Activate() { active = true; }
    public void Deactivate() { active = false; }

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

    void Update()
    {
        if (!PlayerManager.i.engineOn) {
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
        var deltaEueler = Time.deltaTime * turnSpeed * inputDir;
        var targetDelta = Quaternion.Euler(deltaEueler);
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
