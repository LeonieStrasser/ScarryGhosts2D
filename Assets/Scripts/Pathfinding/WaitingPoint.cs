using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingPoint : MonoBehaviour
{
    public bool pointIsFree;
    [HideInInspector]
    public Waypoint wp;

    private void Start()
    {
        wp = gameObject.GetComponent<Waypoint>();
    }
}
