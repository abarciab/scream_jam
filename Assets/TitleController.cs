using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour
{
    [SerializeField] Sound ambientSound, buttonClick;
    [SerializeField] GameObject fade;
    [SerializeField] float transitionTime;

    private void Start()
    {
        ambientSound = Instantiate(ambientSound);
        ambientSound.Play();
        buttonClick = Instantiate(buttonClick);
    }

    public void Click()
    {
        buttonClick.Play();
    }

    public void StartGame()
    {
        fade.SetActive(true);
        Click();
        StartCoroutine(TransitionToGame());
    }

    IEnumerator TransitionToGame()
    {
        float timePassed = 0;
        while (timePassed < transitionTime) {
            ambientSound.PercentVolume(Mathf.Lerp(1, 0, timePassed /  transitionTime));
            yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;
        }
        Destroy(AudioManager.instance.gameObject);
        SceneManager.LoadScene(1);
        //SceneManager.LoadScene(2, LoadSceneMode.Additive);
        SceneManager.LoadScene(3, LoadSceneMode.Additive);
    }
}
