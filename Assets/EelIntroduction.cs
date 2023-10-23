using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EelIntroduction : MonoBehaviour
{
    [SerializeField] Eel eel;
    [SerializeField] Transform eelSummonObj, playerShoveTransform;
    [SerializeField] int eelStartPatrolIndex;
    [SerializeField] float shoveTime = 2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Player>()) Trigger();
    }

    void Trigger()
    {
        GetComponent<Collider>().enabled = false;

        eel.transform.position = eelSummonObj.position;
        eel.gameObject.SetActive(true);
        eel.BigScream();
        eel.SetPatrolIndex(eelStartPatrolIndex);
        StartCoroutine(WaitToPushPlayer());
    }

    IEnumerator WaitToPushPlayer()
    {
        PlayerManager.i.FailEngine();
        while (PlayerManager.i.player.enemiesInContact.Count == 0) yield return new WaitForEndOfFrame();

        var sub = PlayerManager.i.submarine;
        var startPos = sub.position;
        float timePassed = 0;
        while (timePassed < shoveTime) {
            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            sub.transform.position = Vector3.Lerp(startPos, playerShoveTransform.position, timePassed / shoveTime);
        }
        sub.transform.position = playerShoveTransform.position;
    }

}
