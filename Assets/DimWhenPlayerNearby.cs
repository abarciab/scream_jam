using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DimWhenPlayerNearby : MonoBehaviour
{
    [SerializeField] float dimDist;
    Transform player;
    [SerializeField] float checkTick = 1;
    float checkCooldown;
    bool inRange;
    [SerializeField] AnimationCurve dimCurve;
    float maxIntensity;
    [SerializeField] Light lightSource;
    float playerDistance { get { return Vector3.Distance(player.position, transform.position); } }

    private void Start()
    {
        player = PlayerManager.i.submarine;
        inRange = playerDistance < dimDist;
        checkCooldown = Random.Range(0, 10.0f);
        maxIntensity = lightSource.intensity;
    }

    private void Update()
    {
        if (EnvironmentManager.current.globalDim) {
            lightSource.intensity = Mathf.Lerp(lightSource.intensity, 0, 0.05f);
        }
        else if (inRange) {
            Dim();
            return;
        }
        else {
            lightSource.intensity = Mathf.Lerp(lightSource.intensity, maxIntensity, 0.015f);
        }

        checkCooldown -= Time.deltaTime;
        if (checkCooldown < 0) { 
            inRange = playerDistance < dimDist;
            checkCooldown = checkTick;
        }
    }

    void Dim()
    {
        var percent = playerDistance / dimDist;
        if (percent > 1 ) {
            inRange = false;
            return;
        }
        lightSource.intensity = dimCurve.Evaluate( percent ) * maxIntensity;
    }   


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dimDist);
    }
}
