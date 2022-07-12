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
        myPlayer = GetComponentInParent<PlayerMovement>();
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
}
