using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressEvents : MonoBehaviour
{
    ScoreSystem myScoreSystem;
    PlayerMovement myPlayer;
    public GameObject infoUI;

    public int happyGuestsToActivateTeleport = 2;

    private void Awake()
    {
        myScoreSystem = FindObjectOfType<ScoreSystem>();
        myPlayer = FindObjectOfType<PlayerMovement>();
    }

    private void Start()
    {
        myPlayer.backToLobbyIsActivated = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (myScoreSystem.happyGuests == happyGuestsToActivateTeleport)
        {
            if (infoUI.activeSelf == false && myPlayer.backToLobbyIsActivated == false)
            {
                infoUI.SetActive(true);
                myPlayer.backToLobbyIsActivated = true;

            }
        }
    }
}
