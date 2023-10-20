using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AidanTest : MonoBehaviour
{
    private void Update()
    {
        transform.up = PlayerManager.i.submarine.up;
    }
}
