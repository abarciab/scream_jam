using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class EngineDieSharkVisit : MonoBehaviour
{
    [SerializeField] SharkController shark;
    [SerializeField] Vector3 sharkSummonOffset, sharkTargetPos;
    [SerializeField] float sharkWaitTime = 3;
    [SerializeField] Transform sharkEndDest;
    bool triggered;
    Enemy enemy;

    private void Start()
    {
        enemy = shark.GetComponent<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Player>()) Trigger();
    }

    void Trigger()
    {
        if (triggered) return;
        GetComponent<Collider>().enabled = false;

        PlayerManager.i.FailEngine();
        triggered = true;
    }

    private void Update()
    {
        if (triggered && PlayerManager.i.engineOn) StartCoroutine(ShowShark());
        if (enemy.aggro && shark.scriptedBehavior) {
            StopAllCoroutines();
            shark.scriptedBehavior = false;
            print("SHARK AGGRO!");
        }
    }

    IEnumerator ShowShark()
    {
        var sub = PlayerManager.i.submarine;
        shark.gameObject.SetActive(true);
        shark.scriptedBehavior = true;
        triggered = false;
        shark.transform.position = sub.TransformPoint(sharkSummonOffset);
        shark.SetTarget(sub.TransformPoint(sharkTargetPos));

        while (shark.distanceToTarget > 1.5f) yield return new WaitForEndOfFrame();

        enemy.alert = true;
        shark.SetTarget(shark.transform.position);

        float timeLeft = sharkWaitTime;
        while (timeLeft > 0) {
            timeLeft -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        print("waiting done!");

        shark.SetTarget(sharkEndDest.position);
        while (shark.distanceToTarget > 0.4f) yield return new WaitForEndOfFrame();
        shark.scriptedBehavior = false;
    }

    private void OnDrawGizmosSelected()
    {
        var sub = FindObjectOfType<SubmarineMovement>(true);
        if (sub) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(sub.transform.TransformPoint(sharkSummonOffset), 2);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(sub.transform.TransformPoint(sharkTargetPos), 2);
        }
    }

}
