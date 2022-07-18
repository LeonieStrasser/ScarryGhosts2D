using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Highscore
{
    public float score = 0f;

    public bool CheckScore(float value)
    {
        if (value > score)
        {
            OverwriteScore(value);
            return true;
        }
        return false;
    }

    public void OverwriteScore(float value)
    {
        score = value;
    }
}
