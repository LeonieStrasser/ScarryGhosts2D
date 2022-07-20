using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFlip : MonoBehaviour
{
    [SerializeField] ParticleSystemRenderer[] vfx;
    [SerializeField] Vector3 flip;
    bool right = true;


   
    public void FlipParticleSystems()
    {
        if (right)
        {
            for (int i = 0; i < vfx.Length; i++)
            {
                vfx[i].flip = flip;
            }
        }
        else
        {
            for (int i = 0; i < vfx.Length; i++)
            {
                vfx[i].flip = Vector3.zero;
            }
        }
        right = !right;
    }
}

