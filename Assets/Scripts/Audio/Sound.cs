
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    
    public AudioClip myClip;

    [Range(0f, 1f)]
    public float myVolume;
    [Range(.1f, 3f)]
    public float myPitch;
    public bool MyLoop;
    [Tooltip("Nur wenn es ein 3D Sound ist.")]
    public float maxDistance;

    [HideInInspector]
    public AudioSource mySource;

}
