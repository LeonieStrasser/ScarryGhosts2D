using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class LooseEvents : MonoBehaviour
{
    //---------- Loose Screen Behörde bestechen
    ScoreSystem myScoreSystem;
    AudioScript audioManager;
    [SerializeField]
    PlayerMovement myPlayer;
    int costs;
    bool firstTimePayed = false;
    public GameObject schmierButton;
    TextMeshProUGUI schmierButtonTMP;
    Button schmierButtonButton;
    public Color textColor;
    public Color inactiveTextColor;
    public Button notPayButton;
    public string schmierButtonText;
    public GameObject notEnoughMoneyText;

    public float firstCostMultiplyer = 0.6f;
    public float costAdd = 50;

    [Header("Win Event")]
    public Button retryButton;

    private void Awake()
    {
        myScoreSystem = FindObjectOfType<ScoreSystem>();
        audioManager = FindObjectOfType<AudioScript>();
    }

    private void Start()
    {
        schmierButtonTMP = schmierButton.GetComponentInChildren<TextMeshProUGUI>();
        schmierButtonButton = schmierButton.GetComponent<Button>();
    }
    public void GameOver()
    {
                
        myScoreSystem.looseWarningScreen.SetActive(false);
        myScoreSystem.LooseScreen.SetActive(true);

        //AUDIO
        audioManager.Play("LooseSound");

        myScoreSystem.loosUnhappyGuestCount = int.MaxValue;

    }

    public void OnWarningUIActive()
    {
        myPlayer.SwitchActionMap("UI");

        if (!firstTimePayed) // Beim ersten Mal wird der Preis anhand des erwirtschafteten Geldes errechnet
        {
            costs = Mathf.RoundToInt(myScoreSystem.scoreAmount * firstCostMultiplyer) + 1;
            Debug.Log("First Costs are " + costs);
            firstTimePayed = true;

            schmierButtonText = (schmierButtonText + costs + "$").ToString();
            schmierButtonTMP.text = schmierButtonText;
        }

        // Buttons Updaten
        if (myScoreSystem.scoreAmount >= costs)
        {
            schmierButtonButton.interactable = true;
            schmierButtonTMP.color = textColor; ;
            schmierButtonButton.Select();
            notEnoughMoneyText.SetActive(false);
        }
        else
        {
            schmierButtonButton.interactable = false;
            schmierButtonTMP.color = inactiveTextColor;
            notPayButton.Select();
            notEnoughMoneyText.SetActive(true);
        }
    }
    public void BehördeSchmieren()
    {
        myScoreSystem.scoreAmount = myScoreSystem.scoreAmount - costs; // Zahle das Schmiergeld
        costs = Mathf.RoundToInt(costs + costAdd); // Zähle den Preis fürs nächste mal höher
        myScoreSystem.looseWarningScreen.SetActive(false); // Spiel geht weiter
        myScoreSystem.ResetUnhappyGuestCount();


        //Game weiter laufen lassen
        GameManager.Instance.GameRun();
        myPlayer.SwitchActionMap("Player");
    }


    public void OnWinscreenActive()
    {
        myPlayer.SwitchActionMap("UI");

        retryButton.Select();
    }

}
