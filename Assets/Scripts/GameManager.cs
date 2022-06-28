using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{



    // Game states
    enum gamestate { playmode, selectionmode }
    [SerializeField]
    gamestate currentGamestate;

    // Game Time
    public int dayCycle = 10; // wie viele Sekunden hat ein Ingame Tag? (@ Josh)

    // Paths
    [HideInInspector]
    public Pathfinder pathCenter;
    [Tooltip("Hier spawnen NPCs (AUf diesem Objekt muss ein NPC Spawner Script liegen.)")]
    public GameObject spawnpoint;
    [Tooltip("Hier kommt der Player in der Lobby an. Von hier aus geht er zu einem Warteplatz.")]
    public GameObject arrivingPoint;
    //public WaitingPoint nextFreeWaitingPoint;
    [Tooltip("Dies sind die Punkte in der Lobby an denen NPCs auf ihre Zuteilung warten.")]
    public WaitingPoint[] allWaitingPoints;
    // int waitingPointIndex;

    // Gäste
    public List<GameObject> waitingNPCs;

    // Rooms
    public GameObject[] allRooms;
    public List<GameObject> freeRooms;

    // Selection
    [HideInInspector]
    public Selection selectionScript;

    //UI
    [SerializeField]
    HUD_Manager hudMan;

    [Space(30)]
    [Header("Sprite Layer")]
    // Sprite Layer
    public int wandQuerschnittLayer = 1000;
    public int flurObjectsVorPlayerLayer = 900;
    public int playerFlurLayer = 800;
    public int npcFlurLayer = 700;
    public int treppenLayer = 600;
    public int playerBehindTreppe = 500;
    public int flurEnvironmentLayer = 400;
    public int roomFrontLayer = 300;
    public int playerInRoomLayer = 200;
    public int npcInRoomLayer = 100;
    public int roomEnvironment = 0;
    public int roomBackside = -100;
    public int flurBackside = -200;

    private void Awake()
    {
        waitingNPCs = new List<GameObject>();
        allRooms = GameObject.FindGameObjectsWithTag("Room");
        freeRooms = new List<GameObject>();
        pathCenter = GetComponent<Pathfinder>();
        selectionScript = GetComponent<Selection>();
    }


    /// <summary>
    /// NPCs can use this Method to Add themselves to the List of waiting Guests in the Lobby
    /// </summary>
    public void AddMeToWaitingList(GameObject npcObject)
    {
        waitingNPCs.Add(npcObject);
    }

    public void OrderWaitinglistByX()
    {
        waitingNPCs = waitingNPCs.OrderBy(o => o.transform.position.x).ToList();
    }

    public void RemoveMeFromWaitingList(GameObject npcObject)
    {
        for (int i = 0; i < waitingNPCs.Count; i++)
        {
            if (waitingNPCs[i].gameObject.name == npcObject.name)
            {
                waitingNPCs.RemoveAt(i);
                break;
            }
        }
    }


    public void ChangeGameMode()
    {
        switch (currentGamestate)
        {
            case gamestate.playmode: // Change to SELECTIONMODE


                UpdateFreeRooms();
                currentGamestate = gamestate.selectionmode;
                selectionScript.enabled = true;
                break;
            case gamestate.selectionmode: // Change to PLAYMODE

                currentGamestate = gamestate.playmode;
                selectionScript.enabled = false;
                break;
            default:
                break;
        }
    }

    public void UpdateFreeRooms()
    {
        freeRooms.Clear();

        for (int i = 0; i < allRooms.Length; i++)
        {
            Room currentRoomToCheck = allRooms[i].GetComponent<Room>();
            if (currentRoomToCheck.free == true)
            {
                freeRooms.Add(currentRoomToCheck.gameObject);
            }
        }
    }

    public bool IsPlayModeOn()
    {
        if (currentGamestate == gamestate.playmode)
        {
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// Gibt den letzten platz der Warteschlange aus (wenn zwei npcs warten, dann den 3. - ist die lobby leer, den 1.)
    /// </summary>
    /// <param name="lobbyFull"></param>
    /// <returns></returns>
    public Waypoint GetNextWaitingpoint(out int waitIndex)
    {
        waitIndex = -1;

        WaitingPoint nextFree = allWaitingPoints[allWaitingPoints.Length - 1];

        waitIndex = -1;

        int index = allWaitingPoints.Length - 1;

        while (index >= 0)                                                                  // Jeder Wartepunkt wird von hinten nach vorne durchgecheckt. ist einer frei, wir er als neuer vorderster Punkt gesetzt.
        {
            if (allWaitingPoints[index].pointIsFree)
            {
                nextFree = allWaitingPoints[index];
                waitIndex = index;
            }
            index--;
        }

        if (waitIndex != -1)
        {
            nextFree.pointIsFree = false;

            return nextFree.gameObject.GetComponent<Waypoint>();

        }
        else
        {
            return null;
        }


    }

    #region inputFUnktions





    #endregion
}
