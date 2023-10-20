using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Sound Scream;
    [SerializeField] Vector2 screamWaitRange = new Vector2(5, 20);
    float screamCooldown;
    public bool aggro, alert;

    private void Start()
    {
        Scream = Instantiate(Scream);
        screamCooldown = Random.Range(screamWaitRange.x, screamWaitRange.y);
    }

    private void Update()
    {
        screamCooldown -= Time.deltaTime;
        if (screamCooldown > 0) return;
        
        Scream.Play(transform);
        screamCooldown = Random.Range(screamWaitRange.x, screamWaitRange.y);
    }
}
