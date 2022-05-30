using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPC_Movement))]
public class Ghost : MonoBehaviour
{
    // Movement
    [SerializeField]
    NPC_Movement myMovement;

    void Start()
    {
        GoToRandomTarget();
    }



    void GoToRandomTarget()
    {
        myMovement.GoToNewTarget(myMovement.GetRandomWaypoint());
    }
}
