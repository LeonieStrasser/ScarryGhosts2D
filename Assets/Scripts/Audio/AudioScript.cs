using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;


public class AudioScript : MonoBehaviour
{
    public float spacialBlend3dSounds = 0.5f;
    public AudioMixerGroup myMixerGroupSFX;
    
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

    public string PlayOneOfTheseSounds(string[] soundsToChoose)
    {
        int chooseSoundIndex = UnityEngine.Random.Range(0, soundsToChoose.Length);
        Play(soundsToChoose[chooseSoundIndex]);

        return soundsToChoose[chooseSoundIndex];
    }


    // Der NPC muss sich bein awake erst die Sound Liste ziehen, dann die Audiosources initialisieren und dann kann er ingame mit Play3dSoundAtMySource den sound in 3D abspielen
    //public Sound[] Get3dSounds()
    //{
    //    List<Sound> SoundlistCopy = new List<Sound>();

    //    for (int i = 0; i < only3dSounds.Length; i++)
    //    {
    //        Sound newSound = new Sound();
    //        newSound.name = only3dSounds[i].name;
    //        newSound.myClip = only3dSounds[i].myClip;
    //        newSound.myVolume = only3dSounds[i].myVolume;
    //        newSound.myPitch = only3dSounds[i].myPitch;
    //        newSound.MyLoop = only3dSounds[i].MyLoop;
    //        newSound.maxDistance = only3dSounds[i].maxDistance;

    //        SoundlistCopy.Add(newSound);
    //    }

    //    return newSoundListCopy;
    //}

    public void Initialize3dSound(GameObject guestObject, out Sound[] mySounds)
    {
        List<Sound> SoundlistCopy = new List<Sound>();

        for (int i = 0; i < only3dSounds.Length; i++)
        {
            Sound newSound = new Sound();
            newSound.name = only3dSounds[i].name;
            newSound.myClip = only3dSounds[i].myClip;
            newSound.myVolume = only3dSounds[i].myVolume;
            newSound.myPitch = only3dSounds[i].myPitch;
            newSound.MyLoop = only3dSounds[i].MyLoop;
            newSound.maxDistance = only3dSounds[i].maxDistance;

            SoundlistCopy.Add(newSound);
        }

        mySounds = SoundlistCopy.ToArray();

        foreach (var item in SoundlistCopy)
        {
            item.mySource = guestObject.AddComponent<AudioSource>();

            //-----------------EINSTELLUNGEN DES 3D SOUNDS
            item.mySource.spatialBlend = spacialBlend3dSounds; // macht es zum 3d Sound
            item.mySource.outputAudioMixerGroup = myMixerGroupSFX;
            //------------------------------------------------
            item.mySource.clip = item.myClip;
            item.mySource.volume = item.myVolume;
            item.mySource.pitch = item.myPitch;
            item.mySource.loop = item.MyLoop;
            item.mySource.rolloffMode = AudioRolloffMode.Custom;
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

    public string PlayOneOfThese3DSounds(string[] soundsToChoose, Sound[] myOwnSounds)
    {
        int chooseSoundIndex = UnityEngine.Random.Range(0, soundsToChoose.Length);
        Play3dSoundAtMySource(soundsToChoose[chooseSoundIndex], myOwnSounds);

        return soundsToChoose[chooseSoundIndex];
    }
}
