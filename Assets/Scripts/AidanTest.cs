using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AidanTest : MonoBehaviour
{
    [SerializeField] bool testCreate, testReturn;
    ObjectPool<GameObject> pool;
    [SerializeField] float ySpawnPos, spawnRadius;
    GameObject enemy;
    List<GameObject> enemies = new List<GameObject>();

    [Header("EnemyData")]
    [SerializeField] GameObject prefab;

    private void Update()
    {
        if (testCreate && enemy == null) {
            testCreate = false;
        }
        if (testReturn && enemy != null) { 
            testReturn = false;
        }
    }

    private void Start()
    {
        var pos2D = Random.insideUnitCircle * spawnRadius;
        var pos = new Vector3(pos2D.x, ySpawnPos, pos2D.y);
    }

    private void OnMouseDown()
    {
        
    }
}
