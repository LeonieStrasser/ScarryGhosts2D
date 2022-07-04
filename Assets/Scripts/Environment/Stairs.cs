using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    [SerializeField]
    GameObject colliderObject;
    [SerializeField]
    GameObject upperGroundCollider;
   

    int stairLayer;
    int inactiveLayer;

    
    public bool directionFlipped;
    


    private void Start()
    {
        stairLayer = LayerMask.NameToLayer("Stairs");
        inactiveLayer = LayerMask.NameToLayer("Inactive");
    }

    public void SetColliderActive()
    {
        colliderObject.layer = stairLayer;
        for (int i = 0; i < colliderObject.transform.childCount; i++)
        {
            colliderObject.transform.GetChild(i).gameObject.layer = stairLayer;
        }

        upperGroundCollider.layer = inactiveLayer;
        for (int i = 0; i < upperGroundCollider.transform.childCount; i++)
        {
            upperGroundCollider.transform.GetChild(i).gameObject.layer = inactiveLayer;
        }
    }
    public void SetColliderInactive()
    {
        colliderObject.layer = inactiveLayer;
        for (int i = 0; i < colliderObject.transform.childCount; i++)
        {
            colliderObject.transform.GetChild(i).gameObject.layer = inactiveLayer;
        }
        upperGroundCollider.layer = stairLayer;
        for (int i = 0; i < upperGroundCollider.transform.childCount; i++)
        {
            upperGroundCollider.transform.GetChild(i).gameObject.layer = stairLayer;
        }
    }


}
