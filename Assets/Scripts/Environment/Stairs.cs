using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    [SerializeField]
    GameObject colliderObject;
    [SerializeField]
    GameObject upperGroundCollider;
    [SerializeField]
    public Transform upperEntrancePoint;

    public void SwitchColliderState()
    {
        if(colliderObject.activeInHierarchy == true)
        {
            colliderObject.SetActive(false);
            upperGroundCollider.SetActive(true);
        }else
        {
            colliderObject.SetActive(true);
            upperGroundCollider.SetActive(false);
        }
    }

    public void SetColliderInactive()
    {
        colliderObject.SetActive(false);
        upperGroundCollider.SetActive(true);
    }
}
