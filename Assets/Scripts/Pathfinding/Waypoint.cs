using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    // public GameObject[] connections;
    public List<GameObject> connections;

    // Specific Properties
    public bool onlyGhosts = false;
    public bool onlyGuests = false;

    // Pathpoint values -------------------

    [HideInInspector]
    public float distanceToMotherPoint;
    [HideInInspector]
    public float wayDistanceUntilThis;
    [HideInInspector]
    public float airDistanceToTarget;
    [HideInInspector]
    public float distanceCosts;

    [HideInInspector]
    public List<Waypoint> wayDescriptionToThis = new List<Waypoint>();

    //--------------------------------------

    private void Start()
    {
        ConnectWaypoint();
    }

    public void CalculateCosts(Waypoint target, Waypoint motherPoint)
    {
        distanceToMotherPoint = Vector3.Distance(transform.position, motherPoint.transform.position);
        wayDistanceUntilThis = motherPoint.wayDistanceUntilThis + distanceToMotherPoint;
        airDistanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        distanceCosts = wayDistanceUntilThis + airDistanceToTarget;


    }

    public void UpdateWaydescription(Waypoint motherPoint)
    {
        for (int i = 0; i < motherPoint.wayDescriptionToThis.Count; i++)
        {
            // Nimm dioe Mother Wegbeschreibung und setze sie in die Wegbeschreibung des Connectionpoints ein
            this.wayDescriptionToThis.Add(motherPoint.wayDescriptionToThis[i]);
        }
        // ADde den mother Point zu meiner Wegbeschreibung
        this.wayDescriptionToThis.Add(motherPoint);
    }

    public void ResetValues()
    {
        distanceToMotherPoint = 0;
        wayDistanceUntilThis = 0;
        airDistanceToTarget = 0;
        distanceCosts = 0;

        CleareWayDescription();
    }
    public void CleareWayDescription()
    {
        wayDescriptionToThis.Clear();
    }


    private void ConnectWaypoint()
    {
        for (int i = 0; i < this.connections.Count; i++) // Schau dir alle meine Connections an
        {
            connections[i].GetComponent<Waypoint>().connections.Add(gameObject);


            int countConnectionsInOther = this.connections[i].GetComponent<Waypoint>().connections.Count - 1; // checke den letzten nicht mit, da das auf jeden Fall ich bin
            for (int t = 0; t < countConnectionsInOther; t++) // So oft wie der andere punkt selber Connections hat und für jede davon
            {
                if (this.gameObject.name == this.connections[i].GetComponent<Waypoint>().connections[t].name) // Checke ob ich mit connection t meiner connection i übereinstimme
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
