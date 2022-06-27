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

    [Tooltip("Treppen von rechts unten nach links oben: 1 - von links unten nach rechts oben: -1")]
    public int stairsDirection = 1;

   

    public void SetColliderActive()
    {
        colliderObject.SetActive(true);
        upperGroundCollider.SetActive(false);
    }
    public void SetColliderInactive()
    {
        colliderObject.SetActive(false);
        upperGroundCollider.SetActive(true);
    }
}
