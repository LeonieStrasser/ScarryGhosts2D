using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   
    

    // Game states
    enum gamestate {playmode, selectionmode}
    [SerializeField]
    gamestate currentGamestate;

    // Paths
    public Pathfinder pathCenter;
    public GameObject spawnpoint;
    public GameObject waitingPoint;

    // G�ste
    public List<GameObject> waitingNPCs;

    // Selection
    Selection selectionScript;

    private void Start()
    {
        waitingNPCs = new List<GameObject>();
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
            case gamestate.playmode:
                currentGamestate = gamestate.selectionmode;
                selectionScript.enabled = true;
                break;
            case gamestate.selectionmode:
                currentGamestate = gamestate.playmode;
                selectionScript.enabled = false;
                break;
            default:
                break;
        }
    }
}
