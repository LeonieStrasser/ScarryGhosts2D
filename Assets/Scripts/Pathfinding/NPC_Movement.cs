using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Movement : MonoBehaviour
{
    // NPC type
    bool friendlyNPC = true;
    bool ghost = false;

    //Movement
    float speed = 2;
    Rigidbody2D rb;


    //Pathfinding Instructions
    public Waypoint spawnPoint; // Da geht der NPC zuerst hin, wenn er spawnt
    public Waypoint startPoint; // Da geht der NPC vom Spawnpunkt hin, und wartet dort

    Waypoint newStartPoint;
    public Waypoint nextTarget;

    // Path
    public Pathfinder pathfinder;
    public Waypoint nextWaypoint;
    List<Waypoint> waypoints = new List<Waypoint>();
    int waypointIndex = 0;

    // Statemachine
    enum NPCState { moving, stop }
    [SerializeField]
    NPCState statemachine;

    private void Start()
    {
        // Zuweisungen
        Rigidbody2D thisRigidbody = GetComponent<Rigidbody2D>();

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GoToNewTarget(nextTarget);
        }
        // -------------------------------------------DEBUG END---------------------------
    }

    private void Move()
    {
        transform.position = Vector2.MoveTowards(this.transform.position, nextWaypoint.transform.position, speed * Time.deltaTime);

        if (this.transform.position == nextWaypoint.transform.position)
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
        LoadPath(spawnPoint, startPoint);
        nextWaypoint = GetWaypoint();
    }
    void GoToNewTarget(Waypoint newTarget)
    {
        waypointIndex = 0;
        statemachine = NPCState.moving;
        LoadPath(newStartPoint, newTarget);
        nextWaypoint = GetWaypoint();
    }

    void LoadPath(Waypoint start, Waypoint target)
    {
        waypoints.Clear();
        waypoints = pathfinder.GetPath(start, target);
    }

    void LoadNextWaypoint()
    {

        waypointIndex++;

        // Der alte Ziel Point muss als neuer Startpoint zwischengespeichert werden - falls von hier ein neuer Weg gefunden werden muss
        newStartPoint = nextWaypoint;

        if (waypointIndex >= waypoints.Count)
        {
            waypointIndex = 0;
            statemachine = NPCState.stop;
        }
    }

    Waypoint GetWaypoint()
    {
        return waypoints[waypointIndex];
    }
}
