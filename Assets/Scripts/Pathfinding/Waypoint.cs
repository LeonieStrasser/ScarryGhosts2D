using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    // public GameObject[] connections;
    public List<GameObject> connections;

    // Pathpoint values -------------------

    public float distanceToMotherPoint;
    public float wayDistanceUntilThis;
    public float airDistanceToTarget;
    public float distanceCosts;

    public List<Transform> wayDescriptionToThis = new List<Transform>();

    //--------------------------------------

    private void Start()
    {
        ConnectWaypoint();
    }

    public void CalculateCosts(Waypoint currentWaypoint, Waypoint target, Waypoint motherPoint)
    {
        distanceToMotherPoint = Vector3.Distance(transform.position, motherPoint.transform.position);
        wayDistanceUntilThis = motherPoint.wayDistanceUntilThis + distanceToMotherPoint;
        airDistanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        distanceCosts = wayDistanceUntilThis + airDistanceToTarget;
    }




    private void ConnectWaypoint()
    {
        for (int i = 0; i < this.connections.Count; i++) // Schau dir alle meine Connections an
        {
            connections[i].GetComponent<Waypoint>().connections.Add(gameObject);


            int countConnectionsInOther = this.connections[i].GetComponent<Waypoint>().connections.Count - 1; // checke den letzten nicht mit, da das auf jeden Fall ich bin
            for (int t = 0; t < countConnectionsInOther; t++) // So oft wie der andere punkt selber Connections hat und f�r jede davon
            {
                if (this.gameObject.name == this.connections[i].GetComponent<Waypoint>().connections[t].name) // Checke ob ich mit connection t meiner connection i �bereinstimme
                {
                    connections[i].GetComponent<Waypoint>().connections.RemoveAt(countConnectionsInOther); // Wenn nein, Adde mich zu der Connection Liste des Punktes hinzu

                }
            }
        }

    }

    private void OnDrawGizmos()
    {
        if (connections.Count > 0)
        {
            foreach (GameObject item in connections)
            {
                Gizmos.DrawLine(transform.position, item.transform.position);
            }
        }

    }
}
