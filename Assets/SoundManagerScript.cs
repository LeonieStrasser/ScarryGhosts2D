using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public static AudioClip spawnNPCSound;
    static AudioSource audioSrc;

    void Start()
    {
        spawnNPCSound = Resources.Load<AudioSource>("spawnNPC");

        audioScr = GetComponent<AudioSource>();

    }

    public static void ploaySound (string clip)
    {
        switch(clip)
        {
            case "spawnNPC":
            audioScr.PlayOneShot(spawnNPCSound);
            break;
        }

    }


}

//SoundManagerScript.PlaySound("spawnNPC")
