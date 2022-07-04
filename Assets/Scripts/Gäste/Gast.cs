using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPC_Movement))]
[RequireComponent(typeof(Collider2D))]
public class Gast : MonoBehaviour
{
    [Header("General")]
    GameManager gm;
    ScoreSystem myScore;
    AudioScript audioManager;

    // Animation
    Animator anim;

    // Movement
    [Header("Movement")]
    [SerializeField]
    NPC_Movement myMovement;

    public float fleeSpeed;

    // Behaviour States
    enum behaviourState { arriving, waitForSelection, angryWaiting, checkin, stayAtRoom, checkout, flee, angryLeaving, findLobbyPlace, lobbyFull, none }                                 // Anmerkung: definiert, wie der Gast mit einem Ziel-Waypoint interagiert, wenn er dort angekommen ist
    [SerializeField]
    behaviourState guestState;

    // Hotel Stats
    [Header("Hotel States")]
    private GameObject myRoom;
    int myCosts;

    [Space(4)]
    [Header("Staying Time")]
    // Staying timer
    public int secondsToStayLeft;
    int npcWillingToStayDays;                                                          // Anmerkung: NPS warten x (Ingame-)Tage (1 Ingametag = dayCycle int)
    public int minStaytime = 1;
    public int maxStaytime = 3;
    public bool timerHasEnded = false;                                                        // Anmerkung: feststellen, ob der Timer beendet wurde

    // angry Waiting
    [Space(4)]
    [Header("Waiting Time")]
    [Tooltip("Sekunden, die Der Gast in der Lobby auf seine Zuweisung wartet.")]
    public int waitingTime = 10;
    [Tooltip("Verbleibende Sekunden ab denen der Gast ein visuelles Feedback gibt, dass er zu lange wartet. (Stufe 1)")]
    public int unhappyTime;
    [Tooltip("Verbleibende Sekunden ab denen der Gast ein visuelles Feedback gibt, dass er kritisch zu lange wartet. (Stufe 2)")]
    public int angryTime;
    int waitingPointIndex;
    public int resetWaitingTime;

    //NPCSelection
    [Space(10)]
    [Header("Selection")]
    public GameObject selectionHover;
    public GameObject selectionEffect;


    [Header("Icon System")]
    public SpriteRenderer iconRenderer;

    public Sprite iconWaitingUnhappy;
    public Sprite iconWaitingAngryIcon;
    public Sprite iconGhostScaredIcon;
    public Sprite iconHappyLeavingIcon;
    public Sprite iconKillScared;

    //Scare Fleeing
    [Header("Fleeing Feedback")]
    [SerializeField]
    GameObject scareMarker;
    [SerializeField]
    GameObject dyingEffect;

    // AUDIO
    [Header("Audio")]
    Sound[] mySounds;
    public float fleeSoundDelay;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        myScore = FindObjectOfType<ScoreSystem>();
        anim = GetComponentInChildren<Animator>();
        audioManager = FindObjectOfType<AudioScript>();

    }
    void Start()
    {

        guestState = behaviourState.arriving;                                                   // Anmerkung: Bis der Timer läuft (erstes Mal myRoom erreicht wurde) ist der Gast im Checkin
        UpdateAnimationState();

        // Timer stellen
        npcWillingToStayDays = Random.Range(minStaytime, maxStaytime);

        secondsToStayLeft = gm.dayCycle * npcWillingToStayDays;                                // Anmerkung: secondsLeft wird errechnet durch den dayCycle und die 
                                                                                               //            NPC Wartezeit (wie lange ist der NPC gewillt zu warten)
        resetWaitingTime = waitingTime;
        mySounds = audioManager.Get3dSounds();
        audioManager.Initialize3dSound(this.gameObject, mySounds);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // Wenn der Collider ein Geist oder Player ist, wird nach seinem Scare-Status gecheckt. Ist er grade Scary muss der NPC fliehen. - Wenn der NPC im Raum ist, checkt er das nicht
        if ((other.tag == "Ghost" || other.tag == "Player" || other.tag == "KillTrigger") && guestState != behaviourState.stayAtRoom)
        {
            ScareTrigger scareScript = other.GetComponent<ScareTrigger>();
            if (scareScript)
            {
                if (scareScript.ScareCheck() == true) // Ist der Collider grade Scary, muss der NPC fliehen
                {
                    StartFleeing();
                }
            }
            else
                Debug.LogWarning("Auf allen Geistern und auf dem Player muss ein Scare-Trigger liegen! " + other.gameObject.name + " hat keinen ScareTrigger!");

            if (other.tag == "Ghost" )
            {
                iconRenderer.enabled = true;
                iconRenderer.sprite = iconGhostScaredIcon;
            }
            else if (other.tag == "KillTrigger")
            {
                iconRenderer.enabled = true;
                iconRenderer.sprite = iconKillScared;
            }
        }

        if(other.tag == "FrontDoor")
        {
            switch (guestState)
            {
                case behaviourState.checkout:
                    audioManager.Play3dSoundAtMySource("GuestLeavesHotel", mySounds);
                    break;
                case behaviourState.flee:
                    audioManager.Play3dSoundAtMySource("GuestLeavesHotelScared", mySounds);
                    break;
                case behaviourState.angryLeaving:
                    audioManager.Play3dSoundAtMySource("GuestLeavesHotelAngry", mySounds);
                    break;
                case behaviourState.arriving:
                case behaviourState.findLobbyPlace:
                    audioManager.Play3dSoundAtMySource("GuestEntersHotel", mySounds);
                    break;
                
                default:
                    break;
            }
        }

    }

    #region rooms



    public void SetNewRoom(GameObject newRoom)
    {
        iconRenderer.enabled = false; // Für den Fall dass ein unhnappy Icon eingeschaltet war

        myRoom = newRoom;
        myCosts = myRoom.GetComponent<Room>().roomPrice;
        Waypoint targetOfMyRoom = newRoom.GetComponent<Room>().myWaypoint;

        myMovement.GoToNewTarget(targetOfMyRoom);                                               // Geht nur solange der Waypoint noch direkt auf dem myRoom Objekt liegr --- sind es später prefabs evtl ändern.

        // Selection UI ausschalten
        selectionEffect.SetActive(false);
    }


    void EnterRoom()
    {
        gameObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = gm.playerInRoomLayer;

        if (guestState == behaviourState.checkin)
        {
            StartStayingTimer();
        }
        guestState = behaviourState.stayAtRoom;
        UpdateAnimationState();

        //AUDIO
        audioManager.Play3dSoundAtMySource("GuestLeavesRoom", mySounds);
    }
    void EnterFloor()
    {
        gameObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = gm.playerFlurLayer;

        //AUDIO
        audioManager.Play3dSoundAtMySource("GuestLeavesRoom", mySounds);
    }
    #endregion

    public void StartFleeing()
    {
        // Fluchtspeed umstellen
        myMovement.speed = fleeSpeed;

        // wenn du im Raum bist, gehe erst auf den Flur
        if (guestState == behaviourState.stayAtRoom)
        {
            EnterFloor();
        }
        // Gehe zum Ausgang
        myMovement.GoToNewTarget(gm.spawnpoint.GetComponent<Waypoint>());

        // Anmerkung: wird der Gast beim Warten in der Lobby erschreckt, loggt er sich aus der Waiting List aus
        if (guestState == behaviourState.waitForSelection || guestState == behaviourState.angryWaiting)
            gm.RemoveMeFromWaitingList(this.gameObject);


        anim.SetTrigger("shock");
        guestState = behaviourState.flee;
        UpdateAnimationState();
        DeactivateAllWaitingFeedback();

        // UI Element muss gespawnt werden
        Instantiate(scareMarker, Vector2.zero, Quaternion.identity).GetComponentInChildren<AlarmMarker>().SetFollowTarget(gameObject.transform);


        //AUDIO
        audioManager.Play3dSoundAtMySource("GuestJumpscare", mySounds);
        //hier muss etwas Zeit vergehen bis der FleeSound einsetzt
        StartCoroutine(fleeSoundTimer());
    }
    IEnumerator fleeSoundTimer()
    {
        yield return new WaitForSeconds(fleeSoundDelay);
        audioManager.Play3dSoundAtMySource("GuestFlee", mySounds);
    }

    public bool IsGuestFleeing()
    {
        if (guestState == behaviourState.flee)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #region waypointInteraction
    public void StartWaypointInteraction()
    {
        switch (guestState)                                                                     // Anmerkung: Je nach State des Gastes interagiert er anders, wenn er einen Zielpunkt erreicht
        {
            case behaviourState.arriving:                                                        //---------> Erstes Mal den eigenen Raum erreichen - NPC tritt ein und startet seinen Timer
                guestState = behaviourState.findLobbyPlace;
                GoAndWaitInLobby();
                UpdateAnimationState();

                break;
            case behaviourState.findLobbyPlace:                                                        //---------> Erstes Mal den eigenen Raum erreichen - NPC tritt ein und startet seinen Timer
                StartWaitingTime();
                // Hier muss sich der NPC in die Waitingselection liste eintragen
                gm.AddMeToWaitingList(this.gameObject);
                gm.selectionScript.UpdateNpcSelection();
                guestState = behaviourState.waitForSelection;
                UpdateAnimationState();

                //AUDIO
                audioManager.Play("LobbyKlingel");
                break;
            case behaviourState.angryLeaving:                                                        //---------> Erstes Mal den eigenen Raum erreichen - NPC tritt ein und startet seinen Timer
                myScore.AddUnhappyGuestCount();
                Despawn();
                break;
            case behaviourState.checkin:                                                        //---------> Erstes Mal den eigenen Raum erreichen - NPC tritt ein und startet seinen Timer
                EnterRoom();

                //AUDIO
                audioManager.Play3dSoundAtMySource("GuestEntersRoom", mySounds);
                break;
            case behaviourState.stayAtRoom:
                break;
            case behaviourState.checkout:                                                       //---------> Den Ausgang erreichen anchdem der Timer abgelaufen ist - NPC despawnt und gibt Punkte auf d. Score
                myScore.AddScore(myCosts);
                myScore.AddHappyGuestCount();
                Despawn();
                break;
            case behaviourState.flee:                                                          //---------> Den Ausgang auf der Flucht erreichen - NPC despawnt und gibt Malus auf d. Score
                myScore.AddUnhappyGuestCount();
                myScore.scaredGuests++;
                if (myRoom)
                    myRoom.GetComponent<Room>().SetDorAsFree(true);
                Despawn();
                break;
            case behaviourState.lobbyFull:
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

    public void Die()
    {
        if (myRoom)
            myRoom.GetComponent<Room>().SetDorAsFree(true);

        // Spawn Scare Trigger + blut Effect
        Instantiate(dyingEffect, transform.position, Quaternion.identity);

        // AUDIO
        audioManager.Play("GuestDeath");

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
        else // Wenn die Warteplätze alle belegt sind
        {
            guestState = behaviourState.lobbyFull;
            myMovement.GoToNewTarget(gm.spawnpoint.GetComponent<Waypoint>());
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

        if (!isNpcAngry)
        {
            guestState = behaviourState.checkin;
            UpdateAnimationState();
        }
        else
        {
            guestState = behaviourState.angryLeaving;
            UpdateAnimationState();
            
        }
            DeactivateAllWaitingFeedback();
    }

    /// <summary>
    /// Wird aufgerufen wenn der Gast per Space in der Lobby ausgewählt wurde
    /// </summary>
    public void OnIAmSelected()
    {
        selectionEffect.SetActive(true);
        selectionHover.SetActive(false);
    }

    

    #region timer

    void StartWaitingTime()
    {

        StartCoroutine(WaitingTimer());                                                      // Anmerkung: TimerTake Coroutine wird gestartet

    }

    public void ResetWaitingTimer()
    {
        waitingTime = resetWaitingTime - 1;
        guestState = behaviourState.waitForSelection;
        UpdateAnimationState();

        iconRenderer.enabled = false;

        DeactivateAllWaitingFeedback();
    }

    void StartStayingTimer()
    {
        if (secondsToStayLeft > 0)
        {
            StartCoroutine(StayTimer());                                                      // Anmerkung: TimerTake Coroutine wird gestartet
        }
        else if (secondsToStayLeft <= 0)
        {
            timerHasEnded = true;
        }
    }


    IEnumerator StayTimer()
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
                UpdateAnimationState();
                EnterFloor();                                                                 // NPC wechselt wieder auf den Flur-Layer
                myRoom.GetComponent<Room>().SetDorAsFree(true);
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


            if (waitingTime == unhappyTime && myRoom == null)                                      // Wenn bei der Unhappy Time noch kein Raum zugeordnet wurde, wird der NPC unhappy und ungeduldig - Stufe 1 - Visuelles Feedback
            {
                guestState = behaviourState.angryWaiting;
                UpdateAnimationState();

                iconRenderer.enabled = true;
                iconRenderer.sprite = iconWaitingUnhappy;

                //AUDIO
                audioManager.Play3dSoundAtMySource("GuestUngeduldig", mySounds);
            }


            if (waitingTime == angryTime && myRoom == null)                                      // Wenn bei der Angry Time noch kein Raum zugeordnet wurde, wird der NPC sauer - Visuelles Feedback
            {
                guestState = behaviourState.angryWaiting;
                UpdateAnimationState();

                iconRenderer.enabled = true;
                iconRenderer.sprite = iconWaitingAngryIcon;

                //AUDIO
                audioManager.Play3dSoundAtMySource("GuestCriticalWaiting", mySounds);
            }

            if (waitingTime <= 0 && gm.selectionScript.GetSelectedNpcName() != this.gameObject.name)            // Wenn nach der waitingTime noch kein Raum zugeordnet wurde, geht der NPC und hinterlässt einen Score-Malus
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

    public void DeactivateAllWaitingFeedback()
    {

        //AUDIO
        audioManager.Stop3dSoundAtMySource("GuestUngeduldig", mySounds);
        audioManager.Stop3dSoundAtMySource("GuestCriticalWaiting", mySounds);
    }
    #endregion

    #region animationBools
    void UpdateAnimationState()
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
                SetFleeAnimation();

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


    void SetIdleAnimation()
    {
        anim.SetBool("idle", true);
        anim.SetBool("move", false);
        anim.SetBool("happyLeaving", false);
        anim.SetBool("selected", false);
        anim.SetBool("angry", false);
        anim.SetBool("flee", false);
    }

    void SetMoveAnimation()
    {
        anim.SetBool("idle", false);
        anim.SetBool("move", true);
        anim.SetBool("happyLeaving", false);
        anim.SetBool("selected", false);
        anim.SetBool("angry", false);
        anim.SetBool("flee", false);
    }

    void SetHappyLeavingAnimation()
    {
        anim.SetBool("idle", false);
        anim.SetBool("move", false);
        anim.SetBool("happyLeaving", true);
        anim.SetBool("selected", false);
        anim.SetBool("angry", false);
        anim.SetBool("flee", false);
    }

    void SetSelectedAnimation()
    {
        anim.SetBool("idle", false);
        anim.SetBool("move", false);
        anim.SetBool("happyLeaving", false);
        anim.SetBool("selected", true);
        anim.SetBool("angry", false);
        anim.SetBool("flee", false);
    }

    void SetAngryAnimation()
    {
        anim.SetBool("idle", false);
        anim.SetBool("move", false);
        anim.SetBool("happyLeaving", false);
        anim.SetBool("selected", false);
        anim.SetBool("angry", true);
        anim.SetBool("flee", false);
    }

    void SetFleeAnimation()
    {
        anim.SetBool("idle", false);
        anim.SetBool("move", false);
        anim.SetBool("happyLeaving", false);
        anim.SetBool("selected", false);
        anim.SetBool("angry", false);
        anim.SetBool("flee", true);
    }
    #endregion
}
