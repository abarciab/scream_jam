using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager current;
    [SerializeField] float combatThreshold;

    private void Awake() { current = this; }

    List<Ruin> ruins = new List<Ruin>();
    [HideInInspector] public List<IntermediateWaypoint> intermediateWaypoints = new List<IntermediateWaypoint>();
    int currentNextRuin;

    [HideInInspector]public List<Transform> monsters = new List<Transform>();
    public bool nextRuinAvaliable { get { return ruins.Count > 0; } }
    public bool inCombat { get { return NumAggroMonsters() > 0; } }
    public int numAlertMonsters { get { return NumAlertMonsters(); } }
    public bool globalDim;

    public void RegisterNewMonster(Transform monster)
    {
        monsters.Add(monster);
    }

    int NumAlertMonsters()
    {
        int num = 0;
        foreach (var m in monsters) {
            if (m && m.GetComponent<Enemy>().alert) num += 1;
        }
        return num;
    }

    int NumAggroMonsters()
    {
        int num = 0;
        foreach (var m in monsters) {
            if (m && m.GetComponent<Enemy>().aggro) num += 1;
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

    public void RemoveMonster(Transform monster)
    {
        if (monsters.Contains(monster)) monsters.Remove(monster);
    }

    public void RemoveRuin(Ruin completedRuin)
    {
        ruins.Remove(completedRuin);
        currentNextRuin += 1;
    }

    public Vector3 GetNextRuinPos()
    {
        var ruinPos = ruins.Count > 0 ? ruins[0].GetPosition() : Vector3.zero;
        if (intermediateWaypoints.Count <= 0) return ruinPos;

        float lowestIndex = Mathf.Infinity;
        IntermediateWaypoint bestPoint = null;
        foreach (var wp in intermediateWaypoints) {
            var si = wp.transform.GetSiblingIndex();
            if (wp.ruinId == currentNextRuin && si < lowestIndex) {
                lowestIndex = si;
                bestPoint = wp;   
            }
        }

        return bestPoint == null ? ruinPos : bestPoint.transform.position;
    }

    public string getNextRuinName()
    {
        return ruins.Count > 0 ? ruins[0].GetName(): "unknown beacon";
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
