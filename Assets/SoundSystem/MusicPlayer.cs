using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] Sound normalMusic, combatMusic, ambientWind;
    bool fadingOut;
    [SerializeField] bool muted, inCombat;

    private void Start()
    {
        normalMusic = Instantiate(normalMusic);
        combatMusic = Instantiate(combatMusic);
        ambientWind = Instantiate(ambientWind);

        normalMusic.Play();
        ambientWind.Play();
        combatMusic.PlaySilent();
    }

    public void FadeOut()
    {
        combatMusic.PercentVolume(0, 0.05f);
        normalMusic.PercentVolume(0, 0.05f);
        ambientWind.PercentVolume(0, 0.05f);
        fadingOut = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) {
            muted = !muted;
        }

        if (muted) {
            normalMusic.PercentVolume(0);
            combatMusic.PercentVolume(0);
            ambientWind.PercentVolume(0);
            return;
        }

        if (fadingOut) return;

        if (inCombat) {
            combatMusic.PercentVolume(1, 0.05f);
            normalMusic.PercentVolume(0, 0.05f);
        }
        else {
            combatMusic.PercentVolume(0, 0.05f);
            normalMusic.PercentVolume(1, 0.05f);
        }
    }
}
