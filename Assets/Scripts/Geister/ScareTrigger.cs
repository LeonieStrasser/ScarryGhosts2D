using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ScareTrigger : MonoBehaviour
{
    [SerializeField]
    bool scareMode = true;

    public bool ScareCheck()
    {
        if (scareMode)
            return true;
        else
            return false;
    }
}
