using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;


public class AudioScript : MonoBehaviour
{
    public Sound[] sounds;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (var item in sounds)
        {
            item.mySource = gameObject.AddComponent<AudioSource>();
            item.mySource.clip = item.myClip;
            item.mySource.volume = item.myVolume;
            item.mySource.pitch = item.myPitch;
        }
    }

    public void Play(string clipName)
    {
        Sound soundToPlay = Array.Find(sounds, sound => sound.name == clipName);
        soundToPlay.mySource.Play();
    }
}
