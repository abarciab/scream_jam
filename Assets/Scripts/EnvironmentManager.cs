using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager current;
    [SerializeField] float combatThreshold;

    private void Awake() { current = this; }

    List<Ruin> ruins = new List<Ruin>();
    [HideInInspector]public List<Transform> monsters = new List<Transform>();
    public bool nextRuinAvaliable { get { return ruins.Count > 0; } }
    public bool inCombat { get { return NumAggroMonsters() > 0; } }

    public void RegisterNewMonster(Transform monster)
    {
        monsters.Add(monster);
    }

    int NumAggroMonsters()
    {
        int num = 0;
        foreach (var m in monsters) {
            if (m.GetComponent<Enemy>().aggro) num += 1;
        }
        return num;
    }

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

    float DistanceToMonster()
    {
        float closestDist = Mathf.Infinity;
        var closestMonster = transform;
        foreach (var m in monsters) {
            var dist = Vector3.Distance(Camera.main.transform.position, m.position);
            if (dist < closestDist) {
                closestDist = dist;
                closestMonster = m;
            }
        }
        return closestDist;
    }

    private void Update()
    {
        if (ruins.Count > 0) ruins[0].nextRuin = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Camera.main.transform.position, combatThreshold);
    }
}
