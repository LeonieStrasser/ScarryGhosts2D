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


    [Header("Steuer")]
    public string steuerString;
    bool steuerFällig = false;
    [SerializeField]
    int steuerPrice = 5;
    [SerializeField]
    float timePeriodForSteuern = 10;
    // Timer
    public float timer = 0f;

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
        ResetSteuerTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameIsRunning)
        {
            // Hier werden die Infotexte angezeigt und die entsprechendden Features nach und nach freigeschaltet
            if (!startInfoDone)
            {
                SetSkillActive(ref startInfoDone, infoTextStart);
            }

            if (myScoreSystem.scaredGuests == 1 && !beamInfoDone)
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

            if (myScoreSystem.happyGuests == happyGuestsToActivateWallSkill)
            {
                if (infoUI.activeSelf == false && !myPlayer.canGoThroughWalls)
                {
                    SetSkillActive(ref myPlayer.canGoThroughWalls, infoTextWall);
                }
            }

            //------------------------ Aktivieren und das Steuer system läuft
            //SteuerTimer();
            //if (steuerFällig)
            //{
            //    continueButton.onClick.AddListener(SteuerZahlen);
            //    SetSkillActive(ref steuerFällig, steuerString);
            //    steuerFällig = false;
            //}
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

    public void SteuerZahlen()
    {
        myScoreSystem.scoreAmount -= steuerPrice;
        continueButton.onClick.RemoveListener(SteuerZahlen);
        ResetSteuerTimer();


    }

    void ResetSteuerTimer()
    {
        timer = gm.dayCycle * timePeriodForSteuern;
    }
    void SteuerTimer()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
        else if (timer <= 0)
        {

            steuerFällig = true;
        }
    }
}
