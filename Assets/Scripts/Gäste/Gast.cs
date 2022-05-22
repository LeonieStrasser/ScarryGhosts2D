using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gast : MonoBehaviour
{

    GameManager gm;

    // Movement
    NPC_Movement myMovement;

    // Hotel Stats
    [HideInInspector]
    public GameObject myRoom;

    // Staying timer
    public int secondsToStayLeft;                                                                   
    public int npcWillingToStayDays;                                                          // Anmerkung: NPS warten x (Ingame-)Tage (1 Ingametag = dayCycle int)
    public bool timerHasEnded = false;                                                        // Anmerkung: feststellen, ob der Timer beendet wurde

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        myMovement = gameObject.GetComponent<NPC_Movement>();

        // Timer stellen
        secondsToStayLeft = gm.dayCycle * npcWillingToStayDays;                                // Anmerkung: secondsLeft wird errechnet durch den dayCycle und die 
                                                                                               //            NPC Wartezeit (wie lange ist der NPC gewillt zu warten)
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

        // Wenn es das erste Mal Raumbetreten ist, dann muss hier der Timer starten --> Hier muss noch ein if hin
        StartStayingTimer();

    }

    public void SetNewRoom(GameObject newRoom)
    {
        myRoom = newRoom;
        Waypoint targetOfMyRoom = newRoom.GetComponent<Room>().myWaypoint;

        myMovement.GoToNewTarget(targetOfMyRoom); // Geht nur solange der Waypoint noch direkt auf dem myRoom Objekt liegr --- sind es später prefabs evtl ändern.
    }


    void StartStayingTimer()
    {
        if (secondsToStayLeft > 0)
        {
            StartCoroutine(TimerTake());                                                      // Anmerkung: TimerTake Coroutine wird gestartet
        }
        else if (secondsToStayLeft <= 0)                                                             // Anmerkung: Selbsterklärend
        {
            timerHasEnded = true;
        }
    }



    IEnumerator TimerTake()
    {
        bool takingAway = true;
        while (takingAway == true)
        {
            yield return new WaitForSeconds(1);
            secondsToStayLeft -= 1;
            //textDisplay.text = "00:" + secondsToStayLeft;

            if (secondsToStayLeft <= 0)
            {
                takingAway = false;
                timerHasEnded = true;
            }
        }
    }
}
