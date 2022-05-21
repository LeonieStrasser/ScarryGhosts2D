using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Waypoint myWaypoint;

    public bool free;

    public GameObject doorHighlight;

    private void Start()
    {
        // Sobald der Raum einen Waypoint in der Hiorarchy hat, kann man ihn hier automatisch zuordnen

        free = true;
    }

    public void HighlightDoor()
    {
        doorHighlight.SetActive(true);
    }
    public void LowlightDoor()
    {
        doorHighlight.SetActive(false);
    }
}
