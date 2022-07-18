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

    private void Awake()
    {
        myScore = FindObjectOfType<ScoreSystem>();
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
        SceneManager.LoadScene(index);
    }

    public void SetEndScore()
    {

        // Happy Guests
        int oldHighscoreGuests = myScore.highscoreGuests;
        scoreTextGuests.text = myScore.happyGuests.ToString();
        highScoreTextGuests.text = oldHighscoreGuests.ToString();

        if (myScore.happyGuests > oldHighscoreGuests)
        {
            guestsHighscoreReachedPannel.SetActive(true);
        }
        // Catches
        int oldHighscoreCatches = myScore.highscoreGhostCatches;
        scoreTextCatches.text = myScore.ghostCatches.ToString();
        highScoreTextCatches.text = oldHighscoreCatches.ToString();

        if (myScore.ghostCatches > oldHighscoreCatches)
        {
            catchesHighscoreReachedPannel.SetActive(true);
        }



        myScore.UpdateHighscores();




    }
}
