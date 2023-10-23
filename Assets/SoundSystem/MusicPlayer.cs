using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] List<Sound> tracks = new List<Sound>();
    float timeLeft, waitTime;
    [SerializeField] Vector2 silenceWaitRange = new Vector2(1, 10);
    [SerializeField] Sound ambient, pauseMusic;
    Sound currentMusic;
    int currentIndex;

    [Header("Aggro music")]
    [SerializeField] bool inCombat;
    [SerializeField] Sound combatMusic;

    [Header("Alert")]
    [SerializeField] Sound alertHeartbeat; 
    [SerializeField] float alertVolPercent; 

    private void Start()
    {
        ambient = Instantiate(ambient);
        pauseMusic = Instantiate(pauseMusic);
        alertHeartbeat = Instantiate(alertHeartbeat);

        pauseMusic.PlaySilent();
        ambient.Play();
        alertHeartbeat.PlaySilent();

        for (int i = 0; i < tracks.Count; i++) {
            tracks[i] = Instantiate(tracks[i]);
        }
        combatMusic = Instantiate(combatMusic);
        combatMusic.PlaySilent();
        StartNext();
    }

    public void FadeOutCurrent(float time)
    {
        timeLeft = waitTime = Mathf.Infinity;
        StartCoroutine(FadeOut(time));
    }

    IEnumerator FadeOut(float time)
    {
        float timePassed = 0;
        while (timePassed < time) {
            tracks[currentIndex].PercentVolume(Mathf.Lerp(1, 0, timePassed / time));
            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    void StartNext()
    {
        currentIndex = Random.Range(0, tracks.Count);
        var selected = tracks[currentIndex];
        selected.Play();
        timeLeft = selected.GetClipLength();
        waitTime = Mathf.Infinity;
        currentMusic = selected;
    }

    private void Update()
    {
        bool alert = EnvironmentManager.current.numAlertMonsters > 0;
        alertHeartbeat.PercentVolume(alert ? 1 : 0, 0.01f);

        inCombat = EnvironmentManager.current.inCombat;
        if (inCombat) PlayCombat();
        else PlayNormal(alert);

        if (Time.timeScale == 0) {
            pauseMusic.PercentVolume(1, 0.05f);
        }
        else {
            pauseMusic.PercentVolume(0, 0.1f);
        }
    }

    void PlayCombat()
    {
        currentMusic.PercentVolume(0, 0.1f);
        combatMusic.PercentVolume(1, 0.1f);
    }

    void PlayNormal(bool alert)
    {
        currentMusic.PercentVolume(alert ? alertVolPercent : 1, 0.05f);
        combatMusic.PercentVolume(0, 0.1f);

        timeLeft -= Time.deltaTime;
        if (timeLeft == Mathf.Infinity) waitTime -= Time.deltaTime;

        if (timeLeft < 0) {
            timeLeft = Mathf.Infinity;
            waitTime = Random.Range(silenceWaitRange.x, silenceWaitRange.y);
        }
        if (waitTime <= 0) {
            StartNext();
        }
    }
}
