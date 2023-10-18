using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AidanTest : MonoBehaviour
{
    [SerializeField] float moveSpeed, turnSpeed, moveSmoothness, turnSmoothness;
    Vector3 turnDelta, Movedelta;

    void Update()
    {
        var moveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) moveDir = transform.forward;
        else if (Input.GetKey(KeyCode.S)) moveDir = transform.forward * -1;
        if (Input.GetKey(KeyCode.D)) moveDir += transform.right;
        else if (Input.GetKey(KeyCode.A)) moveDir += transform.right * -1;
        if (Input.GetKey(KeyCode.Space)) moveDir += Vector3.up;
        if (Input.GetKey(KeyCode.LeftShift)) moveDir += Vector3.down;
        moveDir = moveDir.normalized;
        moveDir *= moveSpeed * Time.deltaTime;
        Movedelta = Vector3.Lerp(Movedelta, moveDir, moveSmoothness);
        transform.position += Movedelta;

        var turnDir = Vector3.zero;
        if (Input.GetKey(KeyCode.E)) turnDir = new Vector3(0, 1, 0);
        if (Input.GetKey(KeyCode.Q)) turnDir = new Vector3(0, -1, 0);
        turnDir *= turnSpeed * Time.deltaTime;
        turnDelta = Vector3.Lerp(turnDelta, turnDir, turnSmoothness);
        transform.localEulerAngles += turnDelta;
    }
}
