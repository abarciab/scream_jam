using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;
    public static PlayerManager i { get { return instance; } }
    public bool headlightsOn { get { return headlightParent.activeInHierarchy; } }

    public Transform submarine;
    [HideInInspector] public SubmarineMovement moveScript;
    public GameObject playerObj;
    public GameObject mainCamera;
    public PlayerCameraController playerCameraController;
    [HideInInspector] public Player player;
    public Transform steerCam;

    [Header("CameraShake")]
    [SerializeField] List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();
    [SerializeField] float maxAmplitude;
    [SerializeField] float defaultTime = 0.7f;
    [SerializeField] AnimationCurve shakeCurve;

    [Header("Headlights")]
    [SerializeField] GameObject headlightParent;
    [SerializeField] SwitchController headlightSwitch;

    [Header("Engine")]
    [SerializeField] Sound engineSound;
    [SerializeField] SwitchController engineSwitch;
    public bool engineOn;
    [SerializeField] float engineToggleTime = 2;
    [SerializeField] Vector2 waitTimeRange = new Vector2(10, 20);
    [SerializeField] Sound engineFailSound;
    [SerializeField] GameObject interiorLight, radarUI;
    float failCooldown;

    private void Start()
    {
        moveScript = submarine.GetComponent<SubmarineMovement>();
        engineSound = Instantiate(engineSound);
        engineFailSound = Instantiate(engineFailSound);
        engineSound.PlaySilent();
        failCooldown = Random.Range(waitTimeRange.x, waitTimeRange.y);
    }

    private void Update()
    {
        if (engineOn) failCooldown -= Time.deltaTime;
        if (failCooldown <= 0) FailEngine();
    }

    public void FailEngine()
    {
        ShakeCamera(0.1f, 1f);
        failCooldown = Random.Range(waitTimeRange.x, waitTimeRange.y);
        ToggleEngine(false);
        engineFailSound.Play();
    }

    public void ToggleEngine(bool on)
    {
        engineOn = on;
        StartCoroutine(SetEngine(on));
        headlightSwitch.GetComponent<Interactable>().enabled = on;
        interiorLight.SetActive(on);
        radarUI.SetActive(on);

        if (on) return;
        ToggleHeadlights(false);
        headlightSwitch.ForceOff();
        engineSwitch.ForceOff();
    }

    IEnumerator SetEngine(bool active)
    {
        float target = active ? 1 : 0;
        float start = active ? 0 : 1;
        float timePassed = 0;
        while (timePassed < engineToggleTime) {
            engineSound.PercentVolume(Mathf.Lerp(start, target, timePassed / engineToggleTime));
            yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;
        }
        engineSound.PercentVolume(target);
        engineOn = active;
    }

    public void ToggleHeadlights(bool on)
    {
        headlightParent.SetActive(on);
    }

    void Awake()
    {
        instance = this;
        player = FindObjectOfType<Player>(true);
    }

    public void ShakeCamera(float amount = 0.5f)
    {
        ShakeCamera(amount, defaultTime);
    }

    public void ShakeCamera(float amount, float time)
    {
        foreach (var cam in cameras) {
            var noise = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            StartCoroutine(ShakeCamera(noise, amount * maxAmplitude, time));
        }
    }

    IEnumerator ShakeCamera(CinemachineBasicMultiChannelPerlin noise, float amplitude, float time)
    {
        float timepassed = 0;
        while (timepassed < time) {
            timepassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            float progress = timepassed / time;
            noise.m_AmplitudeGain = shakeCurve.Evaluate(progress) * amplitude;
        }
        noise.m_AmplitudeGain = 0;
    }

    


}
