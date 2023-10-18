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
        if (Input.GetKey(KeyCode.Space)) moveDir = transform.forward;
        if (Input.GetKey(KeyCode.E)) moveDir += transform.right;
        else if (Input.GetKey(KeyCode.Q)) moveDir += transform.right * -1;
        if (Input.GetKey(KeyCode.F)) moveDir += Vector3.up;
        if (Input.GetKey(KeyCode.LeftShift)) moveDir += Vector3.down;
        moveDir = moveDir.normalized;
        moveDir *= moveSpeed * Time.deltaTime;
        Movedelta = Vector3.Lerp(Movedelta, moveDir, moveSmoothness);
        transform.position += Movedelta;

        var turnDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) turnDir = new Vector3(-1, 0, 0);
        if (Input.GetKey(KeyCode.A)) turnDir += new Vector3(0, -1, 0);
        if (Input.GetKey(KeyCode.S)) turnDir += new Vector3(1, 0, 0);
        if (Input.GetKey(KeyCode.D)) turnDir += new Vector3(0, 1, 0);
        turnDir *= turnSpeed * Time.deltaTime;
        turnDelta = Vector3.Lerp(turnDelta, turnDir, turnSmoothness);
        transform.localEulerAngles += turnDelta;
    }
}
