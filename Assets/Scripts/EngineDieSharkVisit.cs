using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineDieSharkVisit : MonoBehaviour
{
    [SerializeField] SharkController shark;
    [SerializeField] Vector3 sharkSummonOffset, sharkTargetPos, sharkExitPos;
    [SerializeField] float sharkWaitTime = 3;
    bool triggered;
    Enemy enemy;
    [SerializeField] Vector2 updatedEngineDieRange = new Vector2(60, 120);

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
        PlayerManager.i.UpdateEngineFailRange(updatedEngineDieRange);

        PlayerManager.i.FailEngine();
        triggered = true;
    }

    private void Update()
    {
        if (triggered && PlayerManager.i.engineOn) StartCoroutine(ShowShark());
        if (enemy.aggro && shark.scriptedBehavior) {
            StopAllCoroutines();
            shark.scriptedBehavior = false;
            shark.fastTurn = false;
            shark.updateRotation = true;
            shark.goToTarget = true;
            EnvironmentManager.current.globalDim = false;
        }
    }

    IEnumerator ShowShark()
    {
        EnvironmentManager.current.globalDim = true;
        var sub = PlayerManager.i.submarine;
        shark.gameObject.SetActive(true);
        shark.transform.position = sub.TransformPoint(sharkSummonOffset);
        shark.SetTarget(sub.TransformPoint(sharkTargetPos));

        shark.scriptedBehavior = true;
        triggered = false;
        shark.fastTurn = true;
        enemy.alert = true;

        while (!shark.atTarget) {
            yield return new WaitForEndOfFrame();
        }

        shark.fastTurn = false;
        shark.goToTarget = false;
        shark.updateRotation = false;

        float timeLeft = sharkWaitTime;
        while (timeLeft > 0) {
            timeLeft -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        shark.goToTarget = true;
        shark.updateRotation = true;
        shark.SetTarget(sub.TransformPoint(sharkExitPos));
        while (!shark.atTarget) yield return new WaitForEndOfFrame();
        shark.scriptedBehavior = false;
        enemy.alert = false;

        EnvironmentManager.current.globalDim = false;
    }

    private void OnDrawGizmosSelected()
    {
        var sub = FindObjectOfType<SubmarineMovement>(true);
        if (sub) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(sub.transform.TransformPoint(sharkSummonOffset), 2);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(sub.transform.TransformPoint(sharkTargetPos), 2);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(sub.transform.TransformPoint(sharkExitPos), 2);
        }
    }

}
