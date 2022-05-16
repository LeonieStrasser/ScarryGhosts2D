using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinder : MonoBehaviour
{
    public GameObject[] wayDescription;

    public Waypoint currentPoint;
    public Waypoint targetPoint;

    //Listen

    public List<Waypoint> openList;
    public List<Waypoint> closedList;
    public List<Waypoint> workingList;

    // Pathpoints
    Waypoint motherPathpoint;


    public bool foundShortestWay;

    private void Start()
    {
        closedList = new List<Waypoint>();
        openList = new List<Waypoint>();
        workingList = new List<Waypoint>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetPath(currentPoint, targetPoint);
        }
    }

    public GameObject[] GetPath(Waypoint start, Waypoint target)
    {
        //Kreiere einen Start Waypoint

        motherPathpoint = start;
        motherPathpoint.CalculateCosts(target, motherPathpoint);
       
        foundShortestWay = false;

        while (!foundShortestWay)
        {
            // Füge den aktuellen motherPoint zur Closed List hinzu
            closedList.Add(motherPathpoint);

            //berechne für jeden Connection Waypoint des MotherPoints die Kosten und füge ihn zur Arbeitsliste hinzu

            foreach (GameObject item in motherPathpoint.connections)
            {
                Waypoint tempWayPoint = item.GetComponent<Waypoint>();
                workingList.Add(tempWayPoint);
            }


            // Prüfe für jeden PathPoint in der Arbeitsliste ob er schon in der open List ist
            for (int w = 0; w < workingList.Count; w++)
            {
                // Hier sollte für einen besseren ABlauf noch ein Vergleich mit der Closed List hin!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                
                if (openList.Count == 0)
                {
                    workingList[w].CalculateCosts(target, motherPathpoint);
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
                            pointExistInOpenList = true;
                        }
                    }
                }

                if (pointExistInOpenList == false)
                {
                    openList.Add(workingList[w]);
                    continue;
                }

                //------------------// NUR ZUM debuggen---------------------------------------------------------------------------------------------------SPÄTER LÖSCHEN!!!
                for (int i = 0; i < motherPathpoint.connections.Count; i++)
                {
                    motherPathpoint.connections[i].gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                }

                Debug.Log("Arbeitsliste: " + workingList.Count);
                Debug.Log("Open List: " + openList.Count);
                Debug.Log("Closed List: " + closedList.Count);

                //------------------//--------------------------------------------------------------------------------------------------------------------

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
                        Debug.Log("Kürzester Weg gefunden!");
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
        return wayDescription;
    }
}
