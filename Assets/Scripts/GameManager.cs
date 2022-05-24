using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   
    

    // Game states
    enum gamestate {playmode, selectionmode}
    [SerializeField]
    gamestate currentGamestate;

    // Game Time
    public int dayCycle = 10; // wie viele Sekunden hat ein Ingame Tag? (@ Josh)

    // Paths
    public Pathfinder pathCenter;
    public GameObject spawnpoint;
    public GameObject waitingPoint;

    // Gäste
    public List<GameObject> waitingNPCs;

    // Rooms
    public GameObject[] allRooms;
    public List<GameObject> freeRooms;

    // Selection
    Selection selectionScript;

    // Sprite Layer
    public int wandQuerschnittLayer = 90;
    public int treppenLayer = 80;
    public int flurObjectsVorPlayerLayer = 70;
    public int playerFlurLayer = 60;
    public int npcFlurLayer = 50;
    public int flurEnvironmentLayer = 40;
    public int roomFrontLayer = 30;
    public int playerInRoomLayer = 20;
    public int npcInRoomLayer = 10;
    public int roomEnvironment = 0;
    public int roomBackside = -10;
    public int flurBackside = -20;

    private void Start()
    {
        waitingNPCs = new List<GameObject>();
        allRooms = GameObject.FindGameObjectsWithTag("Room");
        freeRooms = new List<GameObject>();
        pathCenter = GetComponent<Pathfinder>();
        selectionScript = GetComponent<Selection>();

       
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            ChangeGameMode();
        }
    }

    /// <summary>
    /// NPCs can use this Method to Add themselves to the List of waiting Guests in the Lobby
    /// </summary>
    public void AddMeToWaitingList(GameObject npcObject)
    {
        waitingNPCs.Add(npcObject);
    }

    public void RemoveMeFromWaitingList(GameObject npcObject)
    {
        for (int i = 0; i < waitingNPCs.Count; i++)
        {
            if(waitingNPCs[i].gameObject.name == npcObject.name)
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
}
