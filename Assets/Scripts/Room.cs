using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Waypoint myWaypoint;

    public Room upNeighbour;
    public Room downNeighbour;
    public Room leftNeighbour;
    public Room rightNeighbour;

    public bool free;

    public GameObject doorHighlight;
    public GameObject doorIsFreeHighlight;
    public GameObject doorIsClosedHighlight;

    private void Start()
    {
        // Sobald der Raum einen Waypoint in der Hiorarchy hat, kann man ihn hier automatisch zuordnen

        SetDorAsFree(true);
    }

    public void HighlightDoorAsHovered()
    {
        doorHighlight.SetActive(true);
    }
    public void LowlightDoor()
    {
        doorHighlight.SetActive(false);
    }

    public void SetDorAsFree(bool isItFree)
    {
        doorIsClosedHighlight.SetActive(!isItFree);
        free = isItFree;

        if(!isItFree)
        {
            HighlightDoorAsFree(false);
        }
    }

    public void HighlightDoorAsFree(bool isItFree)
    {
        doorIsFreeHighlight.SetActive(isItFree);
    }
}
