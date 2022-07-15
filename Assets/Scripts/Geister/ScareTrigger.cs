using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ScareTrigger : MonoBehaviour
{
    [SerializeField]
    bool scareMode = true;
    public Transform targetFollow;
    public bool followTarget;

    private void Update()
    {
        if (followTarget)
            gameObject.transform.position = targetFollow.position;
    }
    public bool ScareCheck()
    {
        if (scareMode)
            return true;
        else
            return false;
    }
}
