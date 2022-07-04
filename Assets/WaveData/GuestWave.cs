using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GuestWave : ScriptableObject
{
    
    [Tooltip("Die mindestzeit, die zwischen zwei Spawns vergehen soll.")]
    public int minTime;
    [Tooltip("Die maximalzeit, die zwischen zwei Spawns vergehen soll.")]
    public int maxTime;
    public int waveSize;
}
