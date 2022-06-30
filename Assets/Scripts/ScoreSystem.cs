using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    public TextMeshProUGUI scoreTMP;
    public GameObject winScreen;
    public GameObject looseScreen;
    //public GameObject test;
    public int scoreAmount;
    public int winHappyGuestCount;
    public int loosUnhappyGuestCount;
    public TextMeshPro happyScore;
    public TextMeshPro unhappyScore;
    int happyGuests;
    int unhappyGuests;

    bool hasScored = false;
    bool lostScore = false;

    void Start()
    {
        scoreAmount = 0;
        scoreTMP.text = scoreAmount + " $";
    }

    // Update is called once per frame
    void Update()
    {
        Conditions();
        scoreTMP.text = scoreAmount.ToString();
        happyScore.text = happyGuests.ToString();
        unhappyScore.text = unhappyGuests.ToString();
    }




    void Conditions()
    {
        //Lose Condition
        if (unhappyGuests >= loosUnhappyGuestCount)
        {
            looseScreen.SetActive(true);
        }

        //Win Condition
        if (happyGuests >= winHappyGuestCount)
        {
            winScreen.SetActive(true);
        }
    }

    #region publicFunktions

    public void AddScore()
    {

        hasScored = true;
        scoreAmount += 10;
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
    }
    public void AddUnhappyGuestCount()
    {
        unhappyGuests++;
    }

    #endregion
}
