using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gast : MonoBehaviour
{

    GameManager gm;
    NPC_Movement myMovement;

    GameObject myRoom;
    
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

    public void SetNewRoom(GameObject newRoom)
    {
        myRoom = newRoom;
        myMovement.GoToNewTarget(myRoom.GetComponent<Waypoint>()); // Geht nur solange der Waypoint noch direkt auf dem myRoom Objekt liegr --- sind es später prefabs evtl ändern.
    }
}
