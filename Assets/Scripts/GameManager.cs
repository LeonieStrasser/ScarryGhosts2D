using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> waitingNPCs;

    private void Start()
    {
        waitingNPCs = new List<GameObject>();
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
}
