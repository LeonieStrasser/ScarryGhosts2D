using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinder : MonoBehaviour
{


    //public Waypoint currentPoint;
    //public Waypoint targetPoint;

    //Listen

    public List<Waypoint> openList;
    public List<Waypoint> closedList;
    public List<Waypoint> workingList;

    private Waypoint[] allWaypoints;
    private List<Waypoint> onlyGhostsWaypoints;
    private List<Waypoint> onlyGuestsWaypoints;
    private List<Waypoint> availableWaypointsForGhosts;
    public Waypoint[] AllWaypoints { get => this.allWaypoints; }

    // Pathpoints
    Waypoint motherPathpoint;


    public bool foundShortestWay;




    private void Awake()
    {
        closedList = new List<Waypoint>();
        openList = new List<Waypoint>();
        workingList = new List<Waypoint>();
        // wayDescription = new List<Waypoint>();

        onlyGhostsWaypoints = new List<Waypoint>();
        onlyGuestsWaypoints = new List<Waypoint>();
        availableWaypointsForGhosts = new List<Waypoint>();

        GameObject[] getAllWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        allWaypoints = new Waypoint[getAllWaypoints.Length];
        for (int i = 0; i < getAllWaypoints.Length; i++)
        {
            getAllWaypoints[i].name = "Waypoint " + i;
            allWaypoints[i] = getAllWaypoints[i].GetComponent<Waypoint>();

            if (allWaypoints[i].onlyGhosts)
            {
                onlyGhostsWaypoints.Add(allWaypoints[i]);
            }

            if (allWaypoints[i].onlyGuests)
            {
                onlyGuestsWaypoints.Add(allWaypoints[i]);
            }

            if (!allWaypoints[i].onlyGuests) // Wenn der Waypoint nicht ausschließlich für Gäste ist, füge ihn zur Liste der erreichbaren Punkte für Geister hinzu -> Daraus werden dann die Random Punkte für den Geist kreiert
            {
                availableWaypointsForGhosts.Add(allWaypoints[i]);
            }
        }


    }


    public List<Waypoint> GetPath(Waypoint start, Waypoint target, bool isNpcFriendly) // Der Bool fragt ab ob der Weg für eienn freundlichen Gast oder einen Geist gesucht werden soll
    {
        if (start == target) // Für den Fall, dass start und Ziel der Selbe Punkt ist
        {
            List<Waypoint> d = new List<Waypoint>();
            d.Add(start);
            return d;
        }

        ResetPathfinder();
        // Fülle die Closed List mit den verbotenen Waypoints
        if (isNpcFriendly) // verbuiete den Gästen die Wegpunkte die nur für Geister sind
        {
            for (int i = 0; i < onlyGhostsWaypoints.Count; i++)
            {
                closedList.Add(onlyGhostsWaypoints[i]);
            }
        }
        else // Verbiete den Geistern die Wegpunkte die nur für Gäste sind
        {
            for (int i = 0; i < onlyGuestsWaypoints.Count; i++)
            {
                closedList.Add(onlyGuestsWaypoints[i]);
            }
        }
        //Kreiere einen Start Waypoint

        motherPathpoint = start;
        motherPathpoint.CalculateCosts(target, motherPathpoint);

        foundShortestWay = false;

        while (!foundShortestWay)
        {
            // Füge den aktuellen motherPoint zur Closed List hinzu
            closedList.Add(motherPathpoint);

            //berechne für jeden Connection Waypoint des MotherPoints der nicht in der Closed List steht die Kosten und füge ihn zur Arbeitsliste hinzu

            for (int i = 0; i < motherPathpoint.connections.Count; i++)
            {
                bool connectionIsInClosedList = false;
                for (int io = 0; io < closedList.Count; io++)
                {
                    if (motherPathpoint.connections[i].name == closedList[io].gameObject.name)
                    {
                        connectionIsInClosedList = true;
                    }
                }
                if (connectionIsInClosedList == false)
                {
                    Waypoint tempWayPoint = motherPathpoint.connections[i].GetComponent<Waypoint>();
                    workingList.Add(tempWayPoint);

                }
            }



            // Prüfe für jeden PathPoint in der Arbeitsliste ob er schon in der open List ist
            for (int w = 0; w < workingList.Count; w++)
            {


                if (openList.Count == 0)
                {
                    workingList[w].CalculateCosts(target, motherPathpoint);
                    workingList[w].UpdateWaydescription(motherPathpoint);



                    openList.Add(workingList[w]);
                    continue;
                }

                bool pointExistInOpenList = false;
                for (int i = 0; i < openList.Count; i++)
                {

                    if ((workingList[w].gameObject.name == openList[i].gameObject.name))
                    {
                        // Vergleich der wayDistanceUtilThis // Wenn beide Points den selben Namen haben - Vergleiche, welches den billigeren Weg hat
                        if (workingList[w].wayDistanceUntilThis >= openList[i].wayDistanceUntilThis)
                        {

                            pointExistInOpenList = true;
                        }
                        else
                        {
                            // Is ja schon in der Open List drin, wir müssen nur den Wert ändern
                            // ACHTUNG!!! Hier muss auch noch die Weg Description upgedatet werden!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                            openList[i].CalculateCosts(target, motherPathpoint);
                            openList[i].CleareWayDescription();
                            openList[i].UpdateWaydescription(motherPathpoint);
                            pointExistInOpenList = true;
                        }
                    }

                }

                if (pointExistInOpenList == false)
                {
                    workingList[w].UpdateWaydescription(motherPathpoint);
                    workingList[w].CalculateCosts(target, motherPathpoint);
                    openList.Add(workingList[w]);

                    continue;
                }



            }
            workingList.Clear();

            // Sortiere die Open List der Kostengröße nach
            var tempOpenList = openList.OrderBy(o => o.distanceCosts).ToList();
            openList = (List<Waypoint>)tempOpenList;

            // Ist der Zielpunkt teil der OpenList?

            for (int i = 0; i < openList.Count; i++)
            {

                if (target.gameObject.name == openList[i].gameObject.name)
                {
                    // Wenn ja - Gibt es noch einen Point in der openList mit weniger Kosten?
                    if (i > 0)
                    {
                        //wenn ja mach den billigsten Punkt zum neuen Mother Point
                        motherPathpoint = openList[0];
                        openList.RemoveAt(i);
                        break;
                    }
                    else
                    {
                        foundShortestWay = true;
                        //Debug.Log("Kürzester Weg gefunden!");
                    }
                }
                else
                {
                    //wenn nein mach den billigsten Punkt zum neuen Mother Point
                    motherPathpoint = openList[0];
                    openList.RemoveAt(i);
                    break;
                }
            }
        } //While Schleife
        List<Waypoint> wayDescription = new List<Waypoint>();
        wayDescription = target.wayDescriptionToThis;
        wayDescription.Add(target);
        //------------------// NUR ZUM debuggen---------------------------------------------------------------------------------------------------SPÄTER LÖSCHEN!!!
        for (int i = 0; i < allWaypoints.Length; i++)
        {
            allWaypoints[i].gameObject.GetComponent<SpriteRenderer>().color = Color.grey;
        }

        for (int d = 0; d < openList.Count; d++)
        {
            openList[d].gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        }
        for (int i = 0; i < wayDescription.Count; i++)
        {
            wayDescription[i].gameObject.GetComponent<SpriteRenderer>().color = Color.magenta;

        }
        //------------------//--------------------------------------------------------------------------------------------------------------------




        return wayDescription;
    }

    void ResetPathfinder()
    {
        // Cleare alle Listen für die nächste Runde
        openList.Clear();
        closedList.Clear();
        workingList.Clear();
        //wayDescription.Clear();

        for (int i = 0; i < allWaypoints.Length; i++)
        {
            allWaypoints[i].ResetValues();
        }
    }

    public Waypoint GetRandomWaypoint()
    {
        // Hier muss noch ausgeschlossen werden, dass dem Geist ein Wegpunkt der onlyGuests Punkte zugewiesen wird

        return availableWaypointsForGhosts[Random.Range(0, availableWaypointsForGhosts.Count)];
    }

    public Waypoint GetSoulWaypoint()
    {
        Waypoint[] allExistingPoints = allWaypoints;
        List<Waypoint> allWaypointList = new List<Waypoint>(allExistingPoints);
        for (int i = 0; i < onlyGhostsWaypoints.Count; i++)
        {
            allWaypointList.Remove(onlyGhostsWaypoints[i]);
        }
        for (int i = 0; i < onlyGuestsWaypoints.Count; i++)
        {
            allWaypointList.Remove(onlyGuestsWaypoints[i]);
        }

        return allWaypointList[Random.Range(0, allWaypointList.Count)];
    }
}
