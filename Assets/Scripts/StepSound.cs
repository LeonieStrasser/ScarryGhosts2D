using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StepSound : MonoBehaviour
{
    [SerializeField]
    Sound groundSound;
    [SerializeField]
    Sound stairSound;
    [SerializeField]
    Sound weaponTakeSound;
    [SerializeField]
    Sound weaponTakeAwaySound;
    [SerializeField]
    Sound ghostBeamEnd;
    [SerializeField]
    Sound beamStart;

    [Range(0f, 2f)]
    public float minPitch;
    [Range(0f, 2f)]
    public float maxPitch;

    [Range(0f, 2f)]
    public float minVol;
    [Range(0f, 2f)]
    public float maxVol;

    AudioSource newSource;

    PlayerMovement myPlayer;

    private void Awake()
    {
        newSource = gameObject.AddComponent<AudioSource>();
        groundSound.mySource = newSource;
        stairSound.mySource = newSource;
        weaponTakeSound.mySource = newSource;
        weaponTakeAwaySound.mySource = newSource;
        ghostBeamEnd.mySource = newSource;
        beamStart.mySource = newSource;
        myPlayer = GetComponentInParent<PlayerMovement>();
    }

    private void Start()
    {
        
    }
    public void PlayStepSound()
    {
        PlaySound(groundSound);
    }

    public void PlayStairSound()
    {
        if (myPlayer.grounded && !myPlayer.stairGrounded)
            PlaySound(stairSound);
        else if (myPlayer.stairGrounded)
            PlaySound(stairSound);
    }

    void PlaySound(Sound playSound)
    {
        newSource.clip = playSound.myClip;
        playSound.mySource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        playSound.mySource.volume = UnityEngine.Random.Range(minVol, maxVol);
        playSound.mySource.Play();
    }

    public void PlayWeaponTakeSound()
    {
        newSource.clip = weaponTakeSound.myClip;
        weaponTakeSound.mySource.pitch = weaponTakeSound.myPitch;
        weaponTakeSound.mySource.volume = weaponTakeSound.myVolume;
        weaponTakeSound.mySource.Play();
    }

    public void PlayWeaponTakeAwaySound()
    {
        newSource.clip = weaponTakeAwaySound.myClip;
        weaponTakeAwaySound.mySource.pitch = weaponTakeAwaySound.myPitch;
        weaponTakeAwaySound.mySource.volume = weaponTakeAwaySound.myVolume;
        weaponTakeAwaySound.mySource.Play();
    }

    public void PlayBeamEnd()
    {
        newSource.clip = ghostBeamEnd.myClip;
        ghostBeamEnd.mySource.pitch = ghostBeamEnd.myPitch;
        ghostBeamEnd.mySource.volume = ghostBeamEnd.myVolume;
        ghostBeamEnd.mySource.Play();
    }

    public void PlayBeamBegin()
    {
        newSource.clip = beamStart.myClip;
        beamStart.mySource.pitch = beamStart.myPitch;
        beamStart.mySource.volume = beamStart.myVolume;
        beamStart.mySource.Play();
    }
}
