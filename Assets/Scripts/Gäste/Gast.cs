using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gast : MonoBehaviour
{

    GameManager gm;
    NPC_Movement myMovement;

    public GameObject myRoom;
    
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        myMovement = gameObject.GetComponent<NPC_Movement>();
    }

    public bool DoIHaveARoom()
    {
        if(myRoom)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void EnterRoom()
    {
        gameObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = -9; //--------------HARDCODED!!!!!------ÄNDERN WENN DIE SORTING ORDER STEHT
    }

    public void SetNewRoom(GameObject newRoom)
    {
        myRoom = newRoom;
        Waypoint targetOfMyRoom = newRoom.GetComponent<Room>().myWaypoint;

        myMovement.GoToNewTarget(targetOfMyRoom); // Geht nur solange der Waypoint noch direkt auf dem myRoom Objekt liegr --- sind es später prefabs evtl ändern.
    }
}
