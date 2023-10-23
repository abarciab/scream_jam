using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCoordinator : MonoBehaviour
{
    List<AudioSource> sources = new List<AudioSource>();
    List<AudioSource> pausedSources = new List<AudioSource>();

    public void AddNewSound(Sound sound, bool restart, bool _3D = true)
    {
        var newSource = gameObject.AddComponent<AudioSource>();
        newSource.outputAudioMixerGroup = AudioManager.instance.GetMixer(sound.type);
        newSource.playOnAwake = false;
        if (_3D) newSource.spatialBlend = 1;
        sound.audioSource = newSource;
        sound.Play(transform.parent, restart);
        sources.Add(newSource);
    }

    public void Pause()
    {
        foreach (var s in sources) {
            if (s.isPlaying) {
                s.Pause();
                pausedSources.Add(s);
            }
        }
    }

    public void Resume()
    {
        foreach (var s in pausedSources) s.Play();
        pausedSources.Clear();
    }

}
