using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager current;

    private void Awake() { current = this; }

    [SerializeField] List<Ruin> ruins = new List<Ruin>();
    public bool nextRuinAvaliable { get { return ruins.Count > 0; } }

    public void RegisterNewRuin(Ruin newRuin)
    {
        for (int i = 0; i < ruins.Count; i++) {
            if (ruins[i].num > newRuin.num) {
                ruins.Insert(i, newRuin);
                return;
            }
        }
        ruins.Add(newRuin);
    }

    public void RemoveRuin(Ruin completedRuin)
    {
        ruins.Remove(completedRuin);
    }

    public Vector3 GetNextRuinPos()
    {
        return ruins.Count > 0 ? ruins[0].transform.position : Vector3.zero;
    }

    private void Update()
    {
        if (ruins.Count > 0) ruins[0].nextRuin = true;
    }
}
