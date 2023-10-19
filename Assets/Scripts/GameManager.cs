using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager i;
    private void Awake() { i = this; }

    public int SeaLevel;
}
