using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] float health, maxHealth;
    public float getHealth { get { return health; } }
    public float healthPercent { get { return health / maxHealth; } }

    public List<Transform> enemiesInTrigger = new List<Transform>();
    public List<Transform> enemiesInContact = new List<Transform>();
    SubmarineMovement moveScript;
    [SerializeField] float defaultDisableTime = 3;
    float waitTimeLeft, currentDisableTime;

    [Header("Low Health")]
    [SerializeField] Color lightAlarmColor;
    [SerializeField] Light interiorLight;
    Color originalLightColor;

    [Header("Flare")]
    [SerializeField] GameObject flarePrefab;
    [SerializeField] Transform flareStartPos;
    [SerializeField] float flareSpeed;

    [Header("Sounds")]
    [SerializeField] Sound crashSound;
    [SerializeField] Sound alarmSound;

    private void Start()
    {
        health = maxHealth;
        moveScript = GetComponent<SubmarineMovement>();
        originalLightColor = interiorLight.color;
        alarmSound = Instantiate(alarmSound);
        crashSound = Instantiate(crashSound);
        alarmSound.PlaySilent();
    }

    private void Update()
    {
        float healthProgress = health / maxHealth;
        alarmSound.PercentVolume(1 - healthProgress);
        interiorLight.color = Color.Lerp(originalLightColor, lightAlarmColor, 1 - healthProgress);

        if (Input.GetKeyDown(KeyCode.T)) LaunchFlare();
    }

    void LaunchFlare()
    {
        var newFlare = Instantiate(flarePrefab, flareStartPos);
        newFlare.transform.parent = null;
        newFlare.GetComponent<Flare>().Fire();
        newFlare.GetComponent<Rigidbody>().velocity = flareStartPos.forward * flareSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Enemy>()) enemiesInContact.Add(collision.transform.transform);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (enemiesInContact.Contains(collision.gameObject.transform)) enemiesInContact.Remove(collision.transform.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Enemy>()) enemiesInTrigger.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (enemiesInTrigger.Contains(other.transform)) enemiesInTrigger.Remove(other.transform);
    }

    public void ShakeSub(float amount = 0.5f)
    {
        PlayerManager.i.ShakeCamera(1);
    }

    public void Damage(float damage)
    {
        print("Hit for " + damage + " damage");
        health -= damage;
        if (health <= 0) {
            health = 0;
            Die();
        }
        if (damage <= 0) return;
        
        moveScript.Spin();
        crashSound.Play();
    }

    public void PushSub(Vector3 force,float disableTime = -1)
    {
        if (disableTime == -1) disableTime = defaultDisableTime;
        GetComponent<Rigidbody>().AddForce(force);
        moveScript.enabled = false;
        StartCoroutine(EnableMove(disableTime));
    }

    IEnumerator EnableMove(float time)
    {
        currentDisableTime = time;
        waitTimeLeft = time;
        while (waitTimeLeft > 0) {
            waitTimeLeft -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        waitTimeLeft = 0;
        moveScript.enabled = true;
    }

    public float getWaitTimeLeftPercent()
    {
        return currentDisableTime != 0 ? 1 - waitTimeLeft / currentDisableTime : 0;
    }

    void Die()
    {
        SceneManager.LoadScene(1);
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
        SceneManager.LoadScene(3, LoadSceneMode.Additive);
    }
}


