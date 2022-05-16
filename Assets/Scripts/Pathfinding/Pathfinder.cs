using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinder : MonoBehaviour
{
    public List<Waypoint> wayDescription;

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
        wayDescription = new List<Waypoint>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetPath(currentPoint, targetPoint);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            GetPath(currentPoint, targetPoint);
        }
    }

    public List<Waypoint> GetPath(Waypoint start, Waypoint target)
    {
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
                        Debug.Log("Kürzester Weg gefunden!");
                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!LISTEN MÜSSEN NOCH geCLEARt werden!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!



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

        wayDescription = targetPoint.wayDescriptionToThis;
        wayDescription.Add(targetPoint);
        //------------------// NUR ZUM debuggen---------------------------------------------------------------------------------------------------SPÄTER LÖSCHEN!!!
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
}
