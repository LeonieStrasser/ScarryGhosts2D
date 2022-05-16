using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFollow : MonoBehaviour
{
    //Movement
    float speed = 2;

    Rigidbody2D rb;



    // Path
    public Pathfinding pathfinder;

    public Transform nextWaypoint;

    GameObject[] waypoints;

    int waypointIndex = 0;

    // Statemachine
    enum NPCState { moving, stop}
    [SerializeField]
    NPCState statemachine;

    private void Start()
    {
        // Zuweisungen------------------------------------------
        Rigidbody2D thisRigidbody = GetComponent<Rigidbody2D>();
        LoadPath();


        if (thisRigidbody)
        {
            rb = thisRigidbody;
        }
        else
        {
            Debug.LogWarning("Rigidbody 2D fehlt!");
        }

        Transform tryGetWaypoint = GetWaypoint(); // Vorher MUSS einmal LoadPath ausgeführt werden sonst ist GetWaypoint = null

        if(tryGetWaypoint)
        {
            nextWaypoint = tryGetWaypoint;
        }
        else
        {
            Debug.LogWarning("Execusion Order Warning: Pathfinding STart muss vor Waypointfollow Start ausgeführt werden!");
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

    void LoadPath()
    {
       // waypoints = pathfinder.GetPath();
    }

     void LoadNextWaypoint()
    {

        waypointIndex++;


        if (waypointIndex >= waypoints.Length)
        {
            waypointIndex = 0;
            statemachine = NPCState.stop;
        }
    }

     Transform GetWaypoint()
    {
        return waypoints[waypointIndex].transform;
    }
}
