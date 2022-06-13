using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    [SerializeField]
    GameObject colliderObject;

    public void SwitchColliderState()
    {
        if(colliderObject.activeInHierarchy == true)
        {
            colliderObject.SetActive(false);
        }else
        {
            colliderObject.SetActive(true);
        }
    }
}
