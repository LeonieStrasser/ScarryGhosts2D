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
    enum behaviourState { arriving, checkin, stayAtRoom, checkout, flee, angryLeaving, findLobbyPlace, none }                                 // Anmerkung: definiert, wie der Gast mit einem Ziel-Waypoint interagiert, wenn er dort angekommen ist
    [SerializeField]
    behaviourState guestState;

    // Hotel Stats
    [HideInInspector]
    public GameObject myRoom;

    // Staying timer
    public int secondsToStayLeft;
    int npcWillingToStayDays;                                                          // Anmerkung: NPS warten x (Ingame-)Tage (1 Ingametag = dayCycle int)
    public int minStaytime = 1;
    public int maxStaytime = 3;
    public bool timerHasEnded = false;                                                        // Anmerkung: feststellen, ob der Timer beendet wurde

    // angry Waiting
    [Tooltip("Sekunden, die Der Gast in der Lobby auf seine Zuweisung wartet.")]
    public int waitingTime = 10;
    [Tooltip("Verbleibende Sekunden ab denen der Gast ein visuelles Feedback gibt, dass er zu lange wartet.")]
    public int angryTime = 3;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        myScore = FindObjectOfType<ScoreSystem>();

        guestState = behaviourState.arriving;                                                   // Anmerkung: Bis der Timer läuft (erstes Mal myRoom erreicht wurde) ist der Gast im Checkin

        // Timer stellen
        npcWillingToStayDays = Random.Range(minStaytime, maxStaytime);

        secondsToStayLeft = gm.dayCycle * npcWillingToStayDays;                                // Anmerkung: secondsLeft wird errechnet durch den dayCycle und die 
                                                                                               //            NPC Wartezeit (wie lange ist der NPC gewillt zu warten)

    }

    #region rooms
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


    void EnterRoom()
    {
        gameObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = gm.playerInRoomLayer;

        if (guestState == behaviourState.checkin)
        {
            StartStayingTimer();
        }
        guestState = behaviourState.stayAtRoom;
    }
    void EnterFloor()
    {
        gameObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = gm.playerFlurLayer;

    }
#endregion


    #region waypointInteraction
    public void StartWaypointInteraction()
    {
        switch (guestState)                                                                     // Anmerkung: Je nach State des Gastes interagiert er anders, wenn er einen Zielpunkt erreicht
        {
            case behaviourState.arriving:                                                        //---------> Erstes Mal den eigenen Raum erreichen - NPC tritt ein und startet seinen Timer
                GoAndWaitInLobby();
                guestState = behaviourState.findLobbyPlace;
                break;
            case behaviourState.findLobbyPlace:                                                        //---------> Erstes Mal den eigenen Raum erreichen - NPC tritt ein und startet seinen Timer
                StartWaitingTime();
                gm.UpdateNextWaitingpoint();
                guestState = behaviourState.checkin;
                break;
            case behaviourState.angryLeaving:                                                        //---------> Erstes Mal den eigenen Raum erreichen - NPC tritt ein und startet seinen Timer
                myScore.DecreaseScore();
                Despawn();
                break;
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


    void Despawn()
    {
        Destroy(this.gameObject);
    }
    #endregion

    void GoAndWaitInLobby()
    {
        myMovement.GoToNewTarget(gm.nextFreeWaitingPoint.GetComponent<Waypoint>());
    }

    #region timer

    void StartWaitingTime()
    {

        StartCoroutine(WaitingTimer());                                                      // Anmerkung: TimerTake Coroutine wird gestartet

    }

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

    IEnumerator WaitingTimer()
    {
        bool takingAway = true;
        while (takingAway == true)
        {
            yield return new WaitForSeconds(1);
            waitingTime -= 1;

            if (waitingTime == angryTime && myRoom == null)                                      // Wenn bei der Angry Time noch kein Raum zugeordnet wurde, wird der NPC sauer - Visuelles Feedback
            {
                GetComponentInChildren<SpriteRenderer>().color = Color.red;
            }

            if (waitingTime <= 0)                                             // Wenn nach der waitingTime noch kein Raum zugeordnet wurde, geht der NPC und hinterlässt einen Score-Malus
            {
                takingAway = false;

                if (myRoom == null)
                {
                    guestState = behaviourState.angryLeaving;                                         // Anmerkung: Ist die Staytime abgelaufen, geht der NPC zum AUsgangspunkt um zu deswawnen

                    myMovement.GoToNewTarget(gm.spawnpoint.GetComponent<Waypoint>());
                }
            }

        }
    }
    #endregion
}
