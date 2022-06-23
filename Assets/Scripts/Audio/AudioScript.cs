using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;


public class AudioScript : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioScript instance;

    // Start is called before the first frame update
    void Awake()
    {
        
        // Wenn schon ein Audiomanager Existiert, mach ihn kaputt
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (var item in sounds)
        {
            item.mySource = gameObject.AddComponent<AudioSource>();
            item.mySource.clip = item.myClip;
            item.mySource.volume = item.myVolume;
            item.mySource.pitch = item.myPitch;
            item.mySource.loop = item.MyLoop;
        }
    }

    private void Start()
    {
        Play("Music");
    }

    public void Play(string clipName)
    {
        Sound soundToPlay = Array.Find(sounds, sound => sound.name == clipName);

        if(soundToPlay == null)
        {
            Debug.LogWarning("AudioScript Error: Der Clipname -> " + clipName + " <- kann in der Audio Liste nicht gefunden werden! Habt ihr ihn falsch geschrieben ihr Pappnasen???");
            return;
        }
            soundToPlay.mySource.Play();
    }
}
