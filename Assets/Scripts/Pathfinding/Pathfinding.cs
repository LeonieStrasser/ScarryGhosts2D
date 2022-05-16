using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinding : MonoBehaviour
{
    public GameObject[] wayDescription;

    public Waypoint currentPoint;
    public Waypoint targetPoint;

    //Listen

    public List<PathPoint> openList;
    public List<PathPoint> closedList;
    public List<PathPoint> workingList;

    // Pathpoints
    PathPoint motherPathpoint;


    public bool foundShortestWay;

    private void Start()
    {
        closedList = new List<PathPoint>();
        openList = new List<PathPoint>();
        workingList = new List<PathPoint>();
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
        //Kreiere einen Start Pathpoint aus dem aktuellen Waypoint des NPC
        PathPoint newPoint = new PathPoint();
        newPoint.CreatePathpoint(start, target.transform, newPoint);
        motherPathpoint = newPoint;

        foundShortestWay = false;

        while (!foundShortestWay)
        {
            // Füge den aktuellen motherPoint zur Closed List hinzu
            closedList.Add(motherPathpoint);

        //Erstelle für jeden Connection Waypoint des MotherPoints einen eigenen PathPoint und füge ihn zur Arbeitsliste hinzu
        List<GameObject> motherConnectionsTemporary = motherPathpoint.GetConnectionObjects();
        foreach (GameObject item in motherConnectionsTemporary)
        {
            PathPoint newConnectionPathpoint = new PathPoint();
            newConnectionPathpoint.CreatePathpoint(item.GetComponent<Waypoint>(), target.transform, motherPathpoint);
            workingList.Add(newConnectionPathpoint);
        }


        // Prüfe für jeden PathPoint in der Arbeitsliste ob er schon in der open List ist
        for (int w = 0; w < workingList.Count; w++)
        {
            if (openList.Count == 0)
            {
                openList.Add(workingList[w]);
                continue;
            }

            bool pointExistInOpenList = false;
            for (int i = 0; i < openList.Count; i++)
            {

                if ((workingList[w].myTransform.position.x == openList[i].myTransform.position.x) && (workingList[w].myTransform.position.y == openList[i].myTransform.position.y))
                {
                    // Vergleich der wayDistanceUtilThis // Wenn beide Points an der selben Position sind (z ausgenommen) - Vergleiche, welches den billigeren Weg hat
                    if (workingList[w].GetWayDistanceUntilThis() >= openList[i].GetWayDistanceUntilThis())
                    {
                        closedList.Add(workingList[w]);
                        pointExistInOpenList = true;
                    }
                    else
                    {
                        openList.Add(workingList[w]);
                        openList.RemoveAt(i);
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
            for (int i = 0; i < motherConnectionsTemporary.Count; i++)
            {
                motherConnectionsTemporary[i].GetComponent<SpriteRenderer>().color = Color.green;
            }

            Debug.Log("Arbeitsliste: " + workingList.Count);

            //------------------//--------------------------------------------------------------------------------------------------------------------

        }
        workingList.Clear();

        // Sortiere die Open List der Kostengröße nach
        var tempOpenList = openList.OrderBy(o => o.distanceCosts).ToList();
        openList = (List<PathPoint>)tempOpenList;

        // Ist der Zielpunkt teil der OpenList?

        for (int i = 0; i < openList.Count; i++)
        {

            if ((target.transform.position.x == openList[i].myTransform.position.x) && (target.transform.position.y == openList[i].myTransform.position.y))
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
