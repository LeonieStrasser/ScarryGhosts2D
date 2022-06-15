using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class NPC_Movement : MonoBehaviour
{
    public int id;
    // Game Managere
    GameManager gm;

    // NPC type
    public bool friendlyNPC = true;
    public bool ghost = false;

    Gast gastBehaviour;
    Ghost ghostBehaviour;

    //Movement
    public float speed = 2;
    [SerializeField]
    private Rigidbody2D rb;


    //Pathfinding Instructions
    Waypoint spawnPoint; // Da geht der NPC zuerst hin, wenn er spawnt
    Waypoint arrivingPointInLobby; // Da geht der NPC vom Spawnpunkt hin

    [SerializeField]
    Waypoint newStartPoint;
    public Waypoint nextTarget;

    // Path
    Pathfinder pathfinder;
    private Waypoint nextWaypoint;
    public List<Waypoint> path = new List<Waypoint>();
    public List<Waypoint> backupPath = new List<Waypoint>();
    int waypointIndex = 0;

    // Statemachine
    enum NPCState { moving, stop }
    [SerializeField]
    NPCState statemachine;

    private void Awake()
    {
        // Zuweisungen
        gm = FindObjectOfType<GameManager>();
        spawnPoint = gm.spawnpoint.GetComponent<Waypoint>(); // evtl direkt den Waypoint holen -- auch waitingpoint
        arrivingPointInLobby = gm.arrivingPoint.GetComponent<Waypoint>();
        pathfinder = gm.pathCenter;
    }

    private void Start()
    {
        if (friendlyNPC)                                // Gäste gehen beim spawnen zum Startpunkt in der Lobby
        {
            gastBehaviour = GetComponent<Gast>();
            GoFromSpawnToStartPoint();
        }
        else if (ghost)
        {
            ghostBehaviour = GetComponent<Ghost>();
        }
    }
    private void Update()
    {
        if(Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Application.Quit();
        }

        switch (statemachine)
        {
            case NPCState.moving:
                Move();
                break;
            case NPCState.stop:
                Stop();
                break;
            default:
                break;
        }
    }

    private void Move()
    {
        transform.position = Vector2.MoveTowards(this.transform.position, nextWaypoint.transform.position, speed * Time.deltaTime);

        if ((this.transform.position.x == nextWaypoint.transform.position.x) && (this.transform.position.y == nextWaypoint.transform.position.y))                                             // Unsicher weil Positions sich ändern
        {

            // Wenn der Waypoint erreicht ist, muss auf den nächsten umgeschaltet werden
            LoadNextWaypoint();
            nextWaypoint = GetWaypoint();
        }

    }

    private void Stop()
    {

    }

    void GoFromSpawnToStartPoint()
    {
        statemachine = NPCState.moving;
        LoadPath(spawnPoint, arrivingPointInLobby);
        nextWaypoint = GetWaypoint();
    }

    void LoadPath(Waypoint start, Waypoint target)
    {
        this.path.Clear();
        this.path = new List<Waypoint>(pathfinder.GetPath(start, target, friendlyNPC));
    }

    void LoadNextWaypoint()
    {

        waypointIndex++;

        // Der alte Ziel Point muss als neuer Startpoint zwischengespeichert werden - falls von hier ein neuer Weg gefunden werden muss
        newStartPoint = nextWaypoint;

        if (waypointIndex >= path.Count)
        {
            waypointIndex = 0;


            if (friendlyNPC)
            {

                statemachine = NPCState.stop;

                //if (gastBehaviour.AskForCheckedIn() == false)
                //{
                //    gm.AddMeToWaitingList(this.gameObject);
                //}


            }
            else if (ghost)
            {

            }
            OnTargetReached();
        }
    }

    Waypoint GetWaypoint()
    {
        return path[waypointIndex];
    }

    //----------------------------------PUBLIC METHODES
    public void GoToNewTarget(Waypoint newTarget)
    {
        nextTarget = newTarget;

        waypointIndex = 0;
        statemachine = NPCState.moving;
        LoadPath(newStartPoint, nextTarget);
        nextWaypoint = GetWaypoint();
    }

    /// <summary>
    /// Hier kommt der Code an, wenn der NPC sein Ziel erreicht hat - maybe kann man hier eine Waypoint aktion triggern?
    /// </summary>
    public void OnTargetReached()
    {

        if (friendlyNPC)
        {
            gastBehaviour.StartWaypointInteraction();
        }
        else if (ghost)
        {
            ghostBehaviour.GoToRandomTarget();
        }
    }

    public Waypoint GetRandomWaypoint()
    {
        Waypoint newRandomPoint;
        do
        {
            newRandomPoint = pathfinder.GetRandomWaypoint();
        } while (newRandomPoint == nextWaypoint);


        return newRandomPoint;
    }
}
