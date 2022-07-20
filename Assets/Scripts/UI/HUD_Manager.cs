using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class HUD_Manager : MonoBehaviour
{
    [SerializeField]
    PlayerMovement myPlayer;
    ScoreSystem myScore;
    AudioScript myAudioManager;

    [Header("Selection Mode")]
    [SerializeField]
    GameObject selectionModeUI;
    [SerializeField]
    TextMeshProUGUI selectionModeText;
    public string selectGuestText;
    public string selectRoomText;
    public string noGuests;
    [SerializeField]
    Image aInputSprite;

    [Space(10)]

    [Header("Overview Mode")]
    [SerializeField]
    GameObject overviewModeUI;
    [SerializeField]
    Volume overviewVolume;

    private bool overviewOn;
    public bool OverviewOn
    {
        get
        {
            return overviewOn;
        }
        set
        {
            overviewOn = value;
            if (value == true)
                EnableOverviewModeUI();
            else
                DisableOverviewModeUI();

        }
    }

    [Space(10)]

    [Header("Other UI")]
    [SerializeField]
    GameObject backTooltipUI;
    [SerializeField]
    GameObject pauseUI;
    [SerializeField]
    Button continueButton;

    // ENdscore
    [Header("Endscore")]
    [SerializeField]
    TextMeshProUGUI scoreTextGuests;
    [SerializeField]
    TextMeshProUGUI highScoreTextGuests;
    public GameObject guestsHighscoreReachedPannel;
    [SerializeField]
    TextMeshProUGUI scoreTextCatches;
    [SerializeField]
    TextMeshProUGUI highScoreTextCatches;
    public GameObject catchesHighscoreReachedPannel;
    [SerializeField]
    TextMeshProUGUI scoreTextMoney;
    [SerializeField]
    TextMeshProUGUI highScoreTextMoney;
    public GameObject moneyHighscoreReachedPannel;
    [SerializeField]
    TextMeshProUGUI scoreTextBlood;
    [SerializeField]
    TextMeshProUGUI highScoreTextBlood;
    public GameObject bloodHighscoreReachedPannel;

    private void Awake()
    {
        myScore = FindObjectOfType<ScoreSystem>();
        myAudioManager = FindObjectOfType<AudioScript>();
    }

    public void EnableSelectionModeUI()
    {
        selectionModeUI.SetActive(true);
        backTooltipUI.SetActive(true);
    }

    public void DisableSelectionModeUI()
    {
        selectionModeUI.SetActive(false);
        backTooltipUI.SetActive(false);
    }

    public void SetNPCSelectionUI()
    {
        selectionModeText.text = selectGuestText;
        aInputSprite.enabled = true;
    }

    public void SetRoomSelectionUI()
    {
        selectionModeText.text = selectRoomText;
        aInputSprite.enabled = true;
    }

    public void NoGuestsWaitingUI()
    {
        selectionModeText.text = noGuests;
        aInputSprite.enabled = false;
    }


    public void EnableOverviewModeUI()
    {
        overviewModeUI.SetActive(true);
        backTooltipUI.SetActive(true);
        overviewVolume.enabled = true;
    }

    public void DisableOverviewModeUI()
    {
        overviewModeUI.SetActive(false);
        backTooltipUI.SetActive(false);

        overviewVolume.enabled = false;
    }

    public void ContinueGame()
    {
        GameManager.Instance.GameRun();
        pauseUI.SetActive(false);
        myPlayer.SwitchActionMap("Player");
    }

    public void PauseUIActive()
    {
        pauseUI.SetActive(true);
        continueButton.Select();
        myPlayer.SwitchActionMap("UI");
    }

    public void LoadScene(int index)
    {
        myAudioManager.StopAllSound();
        SceneManager.LoadScene(index);
    }

    public void SetEndScore()
    {

        // Happy Guests
        scoreTextGuests.text = myScore.happyGuests.ToString();
        highScoreTextGuests.text = ((int)myScore.highscoreGuests.score).ToString();

        if (myScore.highscoreGuests.CheckScore(myScore.happyGuests))
        {
            guestsHighscoreReachedPannel.SetActive(true);
            myScore.SaveHighscore(myScore.highscoreGuests, ScoreSystem.highscoreGuestKey);
        }
        // Catches
        scoreTextCatches.text = myScore.ghostCatches.ToString();
        highScoreTextCatches.text = ((int)myScore.highscoreGhostCatches.score).ToString();

        if (myScore.highscoreGhostCatches.CheckScore(myScore.ghostCatches))
        {
            catchesHighscoreReachedPannel.SetActive(true);
            myScore.SaveHighscore(myScore.highscoreGhostCatches, ScoreSystem.highscoreGhostCatchesKey);
        }

        // Money
        scoreTextMoney.text = myScore.scoreAmount.ToString();
        highScoreTextMoney.text = ((int)myScore.highscoreMoney.score).ToString();

        if (myScore.highscoreMoney.CheckScore(myScore.scoreAmount))
        {
            moneyHighscoreReachedPannel.SetActive(true);
            myScore.SaveHighscore(myScore.highscoreMoney, ScoreSystem.highscoreMoneyKey);
        }

        // Blood

        scoreTextBlood.text = myScore.partOfBloodyFurniture.ToString() + "%";
        highScoreTextBlood.text = myScore.highscoreBlood.score.ToString() + "%";

        if (myScore.highscoreBlood.CheckScore(myScore.partOfBloodyFurniture))
        {
            bloodHighscoreReachedPannel.SetActive(true);
            myScore.SaveHighscore(myScore.highscoreBlood, ScoreSystem.highscoreBloodKey);
        }
    }
}
