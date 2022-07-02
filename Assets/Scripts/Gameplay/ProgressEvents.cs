using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProgressEvents : MonoBehaviour
{
    ScoreSystem myScoreSystem;
    PlayerMovement myPlayer;
    public GameObject infoUI;
    public TextMeshProUGUI infoText;

    [Header("Start")]
    public string infoTextStart;
    bool startInfoDone = false;

    [Header("Beam Info")]
    public string infoTextBeam;
    bool beamInfoDone = false;

    [Header("Teleport Skill")]
    public int happyGuestsToActivateTeleport = 2;
    public string infoTextTeleport;

    [Header("Wall Skill")]
    public int happyGuestsToActivateWallSkill = 5;
    public string infoTextWall;

    private void Awake()
    {
        myScoreSystem = FindObjectOfType<ScoreSystem>();
        myPlayer = FindObjectOfType<PlayerMovement>();
    }

    private void Start()
    {
        myPlayer.backToLobbyIsActivated = false;
        myPlayer.canGoThroughWalls = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Hier werden die Infotexte angezeigt und die entsprechendden Features nach und nach freigeschaltet
        if(!startInfoDone)
        {
            SetSkillActive(ref startInfoDone, infoTextStart);
        }

        if(myScoreSystem.scaredGuests == 1 && !beamInfoDone)
        {
            SetSkillActive(ref beamInfoDone, infoTextBeam);
        }
        
        if (myScoreSystem.happyGuests == happyGuestsToActivateTeleport)
        {
            if (infoUI.activeSelf == false && myPlayer.backToLobbyIsActivated == false)
            {
                SetSkillActive(ref myPlayer.backToLobbyIsActivated, infoTextTeleport);
            }
        }

        if(myScoreSystem.happyGuests == happyGuestsToActivateWallSkill)
        {
            if(infoUI.activeSelf == false && !myPlayer.canGoThroughWalls)
            {
                SetSkillActive(ref myPlayer.canGoThroughWalls, infoTextWall);
            }
        }
    }

    void SetSkillActive(ref bool skillBool, string infoTextToUse)
    {
        infoUI.SetActive(true);
        infoText.text = infoTextToUse;
        skillBool = true;
    }

    public void ContinueProgress()
    {
        infoUI.SetActive(false);
    }
}
