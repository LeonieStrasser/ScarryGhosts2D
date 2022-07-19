using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProgressEvents : MonoBehaviour
{
    ScoreSystem myScoreSystem;
    GameManager gm;
    AudioScript audioManager;
    PlayerMovement myPlayer;
    public GameObject infoUI;
    public TextMeshProUGUI infoText;
    public Button continueButton;

    int currentGoal;
    [System.Serializable]
    public struct goalData { public int goalGuest; public int goalPrice; }
    [SerializeField]
    goalData[] goalList;
    int goalIndex = 0;

    int endlessGoalAddition = 20;

    [Header("Start")]
    public string infoTextStartModeGuests;
    public string infoTextStartModeBlood;
    bool startInfoDone = false;

    [Header("Zwischenziel erreicht")]
    public string infoTextZwischenziel;

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
        audioManager = FindObjectOfType<AudioScript>();
        gm = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        myPlayer.backToLobbyIsActivated = false;
        myPlayer.canGoThroughWalls = false;

        SetCurrentGoal();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameIsRunning)
        {
            // Hier werden die Infotexte angezeigt und die entsprechendden Features nach und nach freigeschaltet
            if (!startInfoDone)
            {
                if (gm.LevelMode == 0)
                    SetSkillActive(ref startInfoDone, (infoTextStartModeGuests + " " + currentGoal + " paying guests."));
                else if (gm.LevelMode == 1)
                    SetSkillActive(ref startInfoDone, infoTextStartModeBlood);
            }

            // Zwischenbelohnungen
            if (myScoreSystem.happyGuests == currentGoal && gm.LevelMode == 0)
            {
                bool placeholderBool = false;
                SetCurrentGoal(); // goal hochzählen
                SetSkillActive(ref placeholderBool, infoTextZwischenziel + " " + currentGoal + " paying guests.");
                continueButton.onClick.AddListener(GetPrice);
            }

            //Der erste scared gast ist geflohen
            if (myScoreSystem.scaredGuests == 1 && !beamInfoDone)
            {
                SetSkillActive(ref beamInfoDone, infoTextBeam);
            }

            // Teleport nutzbar
            if (myScoreSystem.happyGuests == happyGuestsToActivateTeleport)
            {
                if (infoUI.activeSelf == false && myPlayer.backToLobbyIsActivated == false)
                {
                    SetSkillActive(ref myPlayer.backToLobbyIsActivated, infoTextTeleport);
                }
            }

            // Wall gehen nutzbar
            if (myScoreSystem.happyGuests == happyGuestsToActivateWallSkill)
            {
                if (infoUI.activeSelf == false && !myPlayer.canGoThroughWalls)
                {
                    SetSkillActive(ref myPlayer.canGoThroughWalls, infoTextWall);
                }
            }
        }
    }

    void SetCurrentGoal()
    {

        if (goalIndex < goalList.Length) // Wenn noch weitere ziele übrig sind setze das nächste ziel
        {
            currentGoal = goalList[goalIndex].goalGuest;
            goalIndex++;
        }
        else
        {
            currentGoal += endlessGoalAddition;
        }
    }

    void SetSkillActive(ref bool skillBool, string infoTextToUse)
    {
        infoUI.SetActive(true);
        myPlayer.SwitchActionMap("UI");
        continueButton.Select();
        infoText.text = infoTextToUse;
        skillBool = true;

        GameManager.Instance.GamePause();

        // AUDIO
        audioManager.Play("GameInfo");
    }

    public void ContinueProgress()
    {
        infoUI.SetActive(false);
        myPlayer.SwitchActionMap("Player");

        GameManager.Instance.GameRun();
    }

    public void GetPrice()
    {
        myScoreSystem.AddScore(goalList[goalIndex - 2].goalPrice);
        continueButton.onClick.RemoveListener(GetPrice);
    }
}
