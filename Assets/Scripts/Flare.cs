using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flare : MonoBehaviour
{
    [SerializeField] Light flare;
    [SerializeField] float lifeTime;
    [SerializeField] float brightnessPerMeter = 0.2f, brightnessStart = 20;
    Vector3 startPos;

    public void Fire()
    {
        startPos = transform.position;
        flare.intensity = brightnessStart;
    }

    private void OnTriggerEnter(Collider other)
    {
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        float travelDistance = Vector3.Distance(transform.position, startPos);
        flare.intensity = brightnessStart + travelDistance * brightnessPerMeter;
        StartCoroutine(FadeOut(flare.intensity));
    }

    IEnumerator FadeOut(float intensity)
    {
        float timePassed = 0;
        while (timePassed < lifeTime) {
            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            flare.intensity = Mathf.Lerp(intensity, 0, timePassed / lifeTime);
        }

        Destroy(gameObject);
    }
}
