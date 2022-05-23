using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Movement : MonoBehaviour
{
    public int id;
    // Game Managere
    GameManager gm;

    // NPC type
    bool friendlyNPC = true;
    bool ghost = false;

    Gast gastBehaviour;

    //Movement
    public float speed = 2;
    Rigidbody2D rb;


    //Pathfinding Instructions
    Waypoint spawnPoint; // Da geht der NPC zuerst hin, wenn er spawnt
    Waypoint waitingPoint; // Da geht der NPC vom Spawnpunkt hin, und wartet dort

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

    private void Start()
    {
        // Zuweisungen
        Rigidbody2D thisRigidbody = GetComponent<Rigidbody2D>();
        gm = FindObjectOfType<GameManager>();
        spawnPoint = gm.spawnpoint.GetComponent<Waypoint>(); // evtl direkt den Waypoint holen -- auch waitingpoint
        waitingPoint = gm.waitingPoint.GetComponent<Waypoint>();
        pathfinder = gm.pathCenter;

        if (thisRigidbody)
        {
            rb = thisRigidbody;
        }
        else
        {
            Debug.LogWarning("Rigidbody 2D fehlt!");
        }

        if (friendlyNPC)
        {
            gastBehaviour = GetComponent<Gast>();
            GoFromSpawnToStartPoint();
        }
    }
    private void Update()
    {
      

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

        // -------------------------------------------DEBUG-------------------------------
        if (Input.GetKeyDown(KeyCode.P) && nextTarget)
        {
            GoToNewTarget(nextTarget);
        }
        // -------------------------------------------DEBUG END---------------------------
    }

    private void Move()
    {
        transform.position = Vector2.MoveTowards(this.transform.position, nextWaypoint.transform.position, speed * Time.deltaTime);

        if (this.transform.position == nextWaypoint.transform.position)                                             // Unsicher weil Positions sich ändern
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
        LoadPath(spawnPoint, waitingPoint);
        nextWaypoint = GetWaypoint();
    }

    void LoadPath(Waypoint start, Waypoint target)
    {
        this.path.Clear();
        this.path = new List<Waypoint>(pathfinder.GetPath(start, target));
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

                if (gastBehaviour.DoIHaveARoom() == false)
                {
                    gm.AddMeToWaitingList(this.gameObject);
                }
                else
                {
                    OnTargetReached();
                }
            }
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
        Debug.Log("Waypoint reached: " + nextTarget.gameObject.name);

        if(friendlyNPC)
        {
            gastBehaviour.StartWaypointInteraction();
        }
    }

}
