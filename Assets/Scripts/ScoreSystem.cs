using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;

public class ScoreSystem : MonoBehaviour
{
    GameManager gm;
    AudioScript audioManager;
    LooseEvents looseScript;
    HUD_Manager hudMan;



    public TextMeshProUGUI scoreTMP;
    public GameObject winScreen;
    public GameObject winScreenHighscore;
    public GameObject winScreenKill;
    public GameObject looseWarningScreen;
    public GameObject LooseScreen;
    //public GameObject test;
    public int scoreAmount;
    public int winHappyGuestCount;
    public int loosUnhappyGuestCount;
    public TextMeshPro happyScore;
    public TextMeshPro unhappyScore;
    public int happyGuests;
    public int unhappyGuests;
    public int scaredGuests;

    bool hasScored = false;
    bool lostScore = false;

    // HIGHSCORES
    //-------------------
    public const string highscoreGuestKey = "highscoreGuestKey";
    public const string highscoreGhostCatchesKey = "highscoreGhostCatchesKey";
    public const string highscoreMoneyKey = "highscoreMoneyKey";
    public const string highscoreBloodKey = "highscoreBloodKey";

    public Highscore highscoreGuests;
    public Highscore highscoreGhostCatches;
    public Highscore highscoreMoney;
    public Highscore highscoreBlood;
    //----------------

    [Header("Killmode")]
    [SerializeField]
    private int bloodyFurniture = 0;
    public int BloodyFurniture
    {
        get
        {
            return bloodyFurniture;
        }
        set
        {
            bloodyFurniture = value;
            partOfBloodyFurniture = ((float)bloodyFurniture / (float)frontFurnitureCount) * 100;
            Debug.Log(bloodyFurniture + " / " + frontFurnitureCount + "*100 = " + partOfBloodyFurniture);
        }
    }
    int frontFurnitureCount;
    public float partOfBloodyFurniture = 0;

    public int guestKills = 0;

    public int ghostCatches = 0;

    [SerializeField]
    TextMeshProUGUI bloodCount;
    [SerializeField]
    GameObject killModeUi;

    private void Awake()
    {
        looseScript = FindObjectOfType<LooseEvents>();
        gm = FindObjectOfType<GameManager>();
        audioManager = FindObjectOfType<AudioScript>();
        hudMan = FindObjectOfType<HUD_Manager>();

        highscoreGuests = LoadHighscore(highscoreGuestKey);
        highscoreGhostCatches = LoadHighscore(highscoreGhostCatchesKey);
        highscoreMoney = LoadHighscore(highscoreMoneyKey);
        highscoreBlood = LoadHighscore(highscoreBloodKey);
    }
    void Start()
    {
        scoreAmount = 0;
        scoreTMP.text = scoreAmount.ToString();

        DirtObject[] allFrontFurniture = FindObjectsOfType<DirtObject>();
        frontFurnitureCount = allFrontFurniture.Length;

        if (gm.LevelMode == 1) //wenn killmode on ist
        {
            killModeUi.SetActive(true);
        }
        else
        {
            killModeUi.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameIsRunning)
        {
            Conditions();
            scoreTMP.text = scoreAmount.ToString();
            happyScore.text = happyGuests.ToString();
            unhappyScore.text = unhappyGuests.ToString();

            if (gm.LevelMode == 1) //wenn killmode on ist
            {
                bloodCount.text = bloodyFurniture + " / " + frontFurnitureCount;
            }
        }
    }






    void Conditions()
    {
        if (gm.LevelMode == 0)
        {
            //Lose Condition
            if (unhappyGuests >= loosUnhappyGuestCount)
            {
                looseWarningScreen.SetActive(true);
                looseScript.OnWarningUIActive();

                //Pause
                GameManager.Instance.GamePause();
            }

            //Win Condition
            if (happyGuests >= winHappyGuestCount)
            {


                
            }
        }
        else if (gm.LevelMode == 1)
        {
            // Win Condition
            if (partOfBloodyFurniture >= 100)
            {
                winScreen.SetActive(true);
                winScreenKill.SetActive(true);
                looseScript.OnWinscreenActive();
                //AUDIO
                audioManager.Play("WinSound");
                //Pause
                GameManager.Instance.GamePause();
            }
        }
    }

    public void GameEndedInScoreMode()
    {
        winScreen.SetActive(true);
        winScreenHighscore.SetActive(true);
        hudMan.SetEndScore();

        looseScript.OnWinscreenActive();


        //AUDIO
        audioManager.Play("WinSound");

        //Pause
        GameManager.Instance.GamePause();
    }

    public void SaveHighscore(Highscore scoreData, string key)
    {
        string json = JsonConvert.SerializeObject(scoreData);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    Highscore LoadHighscore(string key)
    {
        string data = PlayerPrefs.GetString(key, "");
        if (data.Equals(""))
        {
            return new Highscore();
        }
        Highscore score = JsonConvert.DeserializeObject<Highscore>(data);
        return score;
    }

    #region publicFunktions

    public void AddScore(int money)
    {

        hasScored = true;
        scoreAmount += money;
        //Instantiate(test, transform.position, Quaternion.identity); //works, but does not show in Game View? idk (will sowas wie +10 oder -10 instantiaten, besseres optisches feedback)
        if (scoreAmount >= 0)
        {
            scoreTMP.color = Color.white;
        }
        hasScored = false;


    }

    public void DecreaseScore()
    {
        lostScore = true;
        scoreAmount -= 10;
        //Instantiate(test, transform.position, Quaternion.identity);
        if (scoreAmount < 0)
        {
            scoreTMP.color = Color.red;
        }
        lostScore = false;
    }

    public void AddHappyGuestCount()
    {
        happyGuests++;

        //AUDIO
        audioManager.Play("Money");
    }
    public void AddUnhappyGuestCount()
    {
        unhappyGuests++;

        //AUDIO
        audioManager.Play("BadPress");
    }
    public void ResetUnhappyGuestCount()
    {
        unhappyGuests = 0;
    }



    #endregion
}
