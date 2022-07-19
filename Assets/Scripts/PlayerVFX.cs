using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFX : MonoBehaviour
{
    [SerializeField] ParticleSystem beamStartVFX;
    [SerializeField] ParticleSystem beamLoopVFX;

    public void PlayBeamStartVFX()
    {
        beamStartVFX.Play(true);
    }

    public void PlayBeamLoopVFX()
    {
        beamLoopVFX.Play(true);
    }

    public void StopBeamLoopVFX()
    {
        beamLoopVFX.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}
