using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    AudioScript audioManager;
    LooseEvents looseScript;
    HUD_Manager hudMan;

    public TextMeshProUGUI scoreTMP;
    public GameObject winScreen;
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
    public int highscoreGuests;
    public int highscoreGhostCatches;
    //----------------

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
            partOfDirtyFurniture = ((float)bloodyFurniture / (float)frontFurnitureCount) * 100;
            Debug.Log(bloodyFurniture + " / " + frontFurnitureCount + "*100 = " + partOfDirtyFurniture);
        }
    }
    int frontFurnitureCount;
    float partOfDirtyFurniture = 0;

    public int guestKills = 0;

    public int ghostCatches = 0;

    private void Awake()
    {
        looseScript = FindObjectOfType<LooseEvents>();
        audioManager = FindObjectOfType<AudioScript>();
        hudMan = FindObjectOfType<HUD_Manager>();
    }
    void Start()
    {
        scoreAmount = 0;
        scoreTMP.text = scoreAmount.ToString();

        DirtObject[] allFrontFurniture = FindObjectsOfType<DirtObject>();
        frontFurnitureCount = allFrontFurniture.Length;
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
        }
    }




    void Conditions()
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
            winScreen.SetActive(true);
            hudMan.SetEndScore();

            looseScript.OnWarningUIActive();
            

            //AUDIO
            audioManager.Play("WinSound");

            //Pause
            GameManager.Instance.GamePause();

            //
        }
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

    public void UpdateHighscores()
    {
        if(happyGuests > highscoreGuests)
        {
            highscoreGuests = happyGuests;
        }

        if(ghostCatches > highscoreGhostCatches)
        {
            highscoreGhostCatches = ghostCatches;
        }
    }

    #endregion
}
