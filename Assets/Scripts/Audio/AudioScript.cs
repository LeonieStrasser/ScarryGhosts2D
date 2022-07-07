using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;


public class AudioScript : MonoBehaviour
{
    public Sound[] sounds;
    public Sound[] only3dSounds;

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
        //Play("Music");
    }

    public void Play(string clipName)
    {
        Sound soundToPlay = Array.Find(sounds, sound => sound.name == clipName);

        if (soundToPlay == null)
        {
            Debug.LogWarning("AudioScript Error: Sound kann nicht abgespielt werden! Der Clipname -> " + clipName + " <- kann in der Audio Liste nicht gefunden werden! Habt ihr ihn falsch geschrieben ihr Pappnasen???");
            return;
        }
        soundToPlay.mySource.Play();
    }



    public void Stop(string clipName)
    {
        Sound soundToPlay = Array.Find(sounds, sound => sound.name == clipName);

        if (soundToPlay == null)
        {
            Debug.LogWarning("AudioScript Error: Sound kann nicht gestoppt werden! Der Clipname -> " + clipName + " <- kann in der Audio Liste nicht gefunden werden! Habt ihr ihn falsch geschrieben ihr Pappnasen???");
            return;
        }
        soundToPlay.mySource.Stop();
    }




    // Der NPC muss sich bein awake erst die Sound Liste ziehen, dann die Audiosources initialisieren und dann kann er ingame mit Play3dSoundAtMySource den sound in 3D abspielen
    public Sound[] Get3dSounds()
    {
        return only3dSounds;
    }

    public void Initialize3dSound(GameObject guestObject, Sound[] myOwnSounds)
    {
        foreach (var item in myOwnSounds)
        {
            item.mySource = guestObject.AddComponent<AudioSource>();

            //-----------------EINSTELLUNGEN DES 3D SOUNDS
            item.mySource.spatialBlend = 1; // macht es zum 3d Sound
            //------------------------------------------------
            item.mySource.clip = item.myClip;
            item.mySource.volume = item.myVolume;
            item.mySource.pitch = item.myPitch;
            item.mySource.loop = item.MyLoop;
            item.mySource.maxDistance = item.maxDistance;
        }
    }
    public void Play3dSoundAtMySource(string clipName, Sound[] myOwnSounds)
    {
        Sound soundToPlay = Array.Find(myOwnSounds, sound => sound.name == clipName);

        if (soundToPlay == null)
        {
            Debug.LogWarning("AudioScript Error: Der Clipname -> " + clipName + " <- kann in der Audio Liste nicht gefunden werden! Habt ihr ihn falsch geschrieben ihr Pappnasen???");
            return;
        }

        if (soundToPlay.mySource != null)
            soundToPlay.mySource.Play();
    }

    public void Stop3dSoundAtMySource(string clipName, Sound[] myOwnSounds)
    {
        Sound soundToStop = Array.Find(myOwnSounds, sound => sound.name == clipName);

        if (soundToStop == null)
        {
            Debug.LogWarning("AudioScript Error: Der Clipname -> " + clipName + " <- kann in der Audio Liste nicht gefunden werden! Habt ihr ihn falsch geschrieben ihr Pappnasen???");
            return;
        }
        if (soundToStop.mySource != null)
            soundToStop.mySource.Stop();
    }
}
