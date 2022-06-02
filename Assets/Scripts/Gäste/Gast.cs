using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPC_Movement))]
public class Gast : MonoBehaviour
{

    GameManager gm;
    ScoreSystem myScore;

    // Movement
    [SerializeField]
    NPC_Movement myMovement;

    // Behaviour States
    enum behaviourState { arriving, waitForSelection, angryWaiting, checkin, stayAtRoom, checkout, flee, angryLeaving, findLobbyPlace, none }                                 // Anmerkung: definiert, wie der Gast mit einem Ziel-Waypoint interagiert, wenn er dort angekommen ist
    [SerializeField]
    behaviourState guestState;

    // Hotel Stats
    private GameObject myRoom;

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
    int waitingPointIndex;

    //NPCSelection
    public GameObject selectionHover;

    // Animation
    Animator anim;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        myScore = FindObjectOfType<ScoreSystem>();

        guestState = behaviourState.arriving;                                                   // Anmerkung: Bis der Timer läuft (erstes Mal myRoom erreicht wurde) ist der Gast im Checkin

        // Timer stellen
        npcWillingToStayDays = Random.Range(minStaytime, maxStaytime);

        secondsToStayLeft = gm.dayCycle * npcWillingToStayDays;                                // Anmerkung: secondsLeft wird errechnet durch den dayCycle und die 
                                                                                               //            NPC Wartezeit (wie lange ist der NPC gewillt zu warten)
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        switch (guestState)
        {
            case behaviourState.arriving:
                SetMoveAnimation();

                break;

            case behaviourState.waitForSelection:
                SetIdleAnimation();

                break;

            case behaviourState.angryWaiting:
                SetAngryAnimation();

                break;

            case behaviourState.checkin:
                SetMoveAnimation();

                break;

            case behaviourState.stayAtRoom:
                SetIdleAnimation();

                break;

            case behaviourState.checkout:
                SetHappyLeavingAnimation();

                break;

            case behaviourState.flee:

                break;

            case behaviourState.angryLeaving:
                SetMoveAnimation();

                break;

            case behaviourState.findLobbyPlace:

                break;

            case behaviourState.none:

                break;
            default:
                break;
        }
    }

    #region rooms
    public bool AskForCheckedIn()
    {
        if (!myRoom && guestState == behaviourState.findLobbyPlace)
        {
            return false;
        }
        else
        {
            return true;
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
                // Hier muss sich der NPC in die Waitingselection liste eintragen
                gm.AddMeToWaitingList(this.gameObject);
                gm.selectionScript.UpdateNpcSelection();
                guestState = behaviourState.waitForSelection;
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
        Waypoint nextFreeWaitingPlace = gm.GetNextWaitingpoint(out int waitIndex);

        if (nextFreeWaitingPlace)
        {
            myMovement.GoToNewTarget(nextFreeWaitingPlace);
            waitingPointIndex = waitIndex;                                                  // Der ArrayIndex des zugeordneten WaitingPoints wird gespecichert um diesen später wieder frei geben zu können. (im Selection Script)
        }
    }

    public void LeaveLobby(bool isNpcAngry)
    {
        if (waitingPointIndex >= 0)
        {
            gm.allWaitingPoints[waitingPointIndex].pointIsFree = true;              // Setzt den AKtuellen Wartepunkt wieder frei
        }
        else
            Debug.LogWarning("Der Index des Waitingpoints wurde falsch in den NPC gespeichert. Er sollte negativ sein, wenn kein Platz mehr für den NPC in der Lobby war.");

        if(!isNpcAngry)
        {
            guestState = behaviourState.checkin;
        }
        else
        {
            guestState = behaviourState.angryLeaving;
        }
    }

    /// <summary>
    /// Wird aufgerufen wenn der Gast per Space in der Lobby ausgewählt wurde
    /// </summary>
    public void OnIAmSelected()
    {
     
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
                guestState = behaviourState.angryWaiting;
            }

            if (waitingTime <= 0 && gm.selectionScript.GetSelectedNpcName() != this.gameObject.name)                                             // Wenn nach der waitingTime noch kein Raum zugeordnet wurde, geht der NPC und hinterlässt einen Score-Malus
            {
                takingAway = false;

                if (myRoom == null)
                {
                                                           // Anmerkung: Ist die Staytime abgelaufen, geht der NPC angry zum AUsgangspunkt um zu deswawnen
                    gm.RemoveMeFromWaitingList(this.gameObject);
                    LeaveLobby(true);
                    // Wenn er grade der selected NPC ist, wird automatisch ein anderer selected

                    if (gm.selectionScript.GetSelectedNpcName() == this.gameObject.name)
                    {
                        gm.selectionScript.SetSelectedNpcNull();
                    }
                    myMovement.GoToNewTarget(gm.spawnpoint.GetComponent<Waypoint>());
                }
            }

        }
    }
    #endregion

    #region animationBools

    void SetIdleAnimation()
    {
        anim.SetBool("idle", true);
        anim.SetBool("move", false);
        anim.SetBool("happyLeaving", false);
        anim.SetBool("selected", false);
        anim.SetBool("angry", false);
    }

    void SetMoveAnimation()
    {
        anim.SetBool("idle", false);
        anim.SetBool("move", true);
        anim.SetBool("happyLeaving", false);
        anim.SetBool("selected", false);
        anim.SetBool("angry", false);
    }

    void SetHappyLeavingAnimation()
    {
        anim.SetBool("idle", false);
        anim.SetBool("move", false);
        anim.SetBool("happyLeaving", true);
        anim.SetBool("selected", false);
        anim.SetBool("angry",false);
    }

    void SetSelectedAnimation()
    {
        anim.SetBool("idle", false);
        anim.SetBool("move", false);
        anim.SetBool("happyLeaving", false);
        anim.SetBool("selected", true);
        anim.SetBool("angry", false);
    }

    void SetAngryAnimation()
    {
        anim.SetBool("idle", false);
        anim.SetBool("move", false);
        anim.SetBool("happyLeaving", false);
        anim.SetBool("selected", false);
        anim.SetBool("angry", true);
    }
    #endregion
}
