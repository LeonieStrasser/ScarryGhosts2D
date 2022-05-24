using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gast : MonoBehaviour
{

    GameManager gm;
    ScoreSystem myScore;

    // Movement
    [SerializeField]
    NPC_Movement myMovement;

    // Behaviour States
    enum behaviourState { checkin, stayAtRoom, checkout, flee, none }                                 // Anmerkung: definiert, wie der Gast mit einem Ziel-Waypoint interagiert, wenn er dort angekommen ist
    [SerializeField]
    behaviourState guestState;

    // Hotel Stats
    [HideInInspector]
    public GameObject myRoom;

    // Staying timer
    public int secondsToStayLeft;
 int npcWillingToStayDays;                                                          // Anmerkung: NPS warten x (Ingame-)Tage (1 Ingametag = dayCycle int)
    public int minStaytime = 10;
    public int maxStaytime = 20;
    public bool timerHasEnded = false;                                                        // Anmerkung: feststellen, ob der Timer beendet wurde


    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        myScore = FindObjectOfType<ScoreSystem>();
       // myMovement = gameObject.GetComponent<NPC_Movement>();
        guestState = behaviourState.checkin;                                                   // Anmerkung: Bis der Timer läuft (erstes Mal myRoom erreicht wurde) ist der Gast im Checkin

        // Timer stellen
        npcWillingToStayDays = Random.Range(minStaytime, maxStaytime);

        secondsToStayLeft = gm.dayCycle * npcWillingToStayDays;                                // Anmerkung: secondsLeft wird errechnet durch den dayCycle und die 
                                                                                               //            NPC Wartezeit (wie lange ist der NPC gewillt zu warten)

    }

    public bool DoIHaveARoom()
    {
        if (myRoom)
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
        Waypoint targetOfMyRoom = newRoom.GetComponent<Room>().myWaypoint;

        myMovement.GoToNewTarget(targetOfMyRoom);                                               // Geht nur solange der Waypoint noch direkt auf dem myRoom Objekt liegr --- sind es später prefabs evtl ändern.
    }



    #region waypointInteraction
    public void StartWaypointInteraction()
    {
        switch (guestState)                                                                     // Anmerkung: Je nach State des Gastes interagiert er anders, wenn er einen Zielpunkt erreicht
        {
            case behaviourState.checkin:                                                        //---------> Erstes Mal den eigenen Raum erreichen - NPC tritt ein und startet seinen Timer
                EnterRoom();
                break;
            case behaviourState.stayAtRoom:
                break;
            case behaviourState.checkout:                                                       //---------> Den Ausgang erreichen anchdem der Timer abgelaufen ist - NPC despawnt und gibt Punkte auf d. Score
                myScore.AddScore();
                Despawn();
                break;
            case behaviourState.flee:                                                          //---------> Den Ausgang auf der Flucht erreichen - NPC despawnt und gibt Malus auf d. Score
                myScore.DecreaseScore();
                Despawn();
                break;
            default:
                break;
        }
    }

    void EnterRoom()
    {
        gameObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = -9;                  //--------------HARDCODED Layerwechsel!!!!!------ÄNDERN WENN DIE SORTING ORDER STEHT

        if (guestState == behaviourState.checkin)
        {
            StartStayingTimer();
        }
        guestState = behaviourState.stayAtRoom;
    }
    void EnterFloor()
    {
        gameObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = 0;                  //--------------HARDCODED Layerwechsel!!!!!------ÄNDERN WENN DIE SORTING ORDER STEHT

    }

    void Despawn()
    {
        Destroy(this.gameObject);
    }
    #endregion

    #region timer
    void StartStayingTimer()
    {
        if (secondsToStayLeft > 0)
        {
            StartCoroutine(TimerTake());                                                      // Anmerkung: TimerTake Coroutine wird gestartet
        }
        else if (secondsToStayLeft <= 0)
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
                guestState = behaviourState.checkout;                                         // Anmerkung: Ist die Staytime abgelaufen, geht der NPC zum AUsgangspunkt um zu deswawnen
                EnterFloor();                                                                 // NPC wechselt wieder auf den Flur-Layer
                myRoom.GetComponent<Room>().free = true;
                myMovement.GoToNewTarget(gm.spawnpoint.GetComponent<Waypoint>());
            }
        }
    }
    #endregion
}
