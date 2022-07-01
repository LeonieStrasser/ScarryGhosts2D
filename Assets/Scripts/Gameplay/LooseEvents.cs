using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class LooseEvents : MonoBehaviour
{
    //---------- Loose Screen Beh�rde bestechen
    ScoreSystem myScoreSystem;
    int costs;
    bool firstTimePayed = false;
    public GameObject schmierButton;
    TextMeshProUGUI schmierButtonTMP;
    Button schmierButtonButton; 
    public string schmierButtonText;
    public GameObject notEnoughMoneyText;

    public float firstCostMultiplyer = 0.6f;
    public float costMultiplyer = 1.2f;

    private void Awake()
    {
        myScoreSystem = FindObjectOfType<ScoreSystem>();
    }

    private void Start()
    {
        schmierButtonTMP = schmierButton.GetComponentInChildren<TextMeshProUGUI>();
        schmierButtonButton = schmierButton.GetComponent<Button>();
    }
    public void OnWarningUIActive()
    {
        if (!firstTimePayed) // Beim ersten Mal wird der Preis anhand des erwirtschafteten Geldes errechnet
        {
            costs = Mathf.RoundToInt(myScoreSystem.scoreAmount * firstCostMultiplyer) + 1;
            Debug.Log("First Costs are " + costs);
            firstTimePayed = true;

            schmierButtonText = (schmierButtonText + costs + "$").ToString();
            schmierButtonTMP.text = schmierButtonText;
        }

        // Buttons Updaten
        if(myScoreSystem.scoreAmount >= costs)
        {
            schmierButtonButton.interactable = true;
            notEnoughMoneyText.SetActive(false);
        }
        else
        {
            schmierButtonButton.interactable = false;
            notEnoughMoneyText.SetActive(true);
        }
    }
    public void Beh�rdeSchmieren()
    {
        myScoreSystem.scoreAmount = myScoreSystem.scoreAmount - costs; // Zahle das Schmiergeld
        costs = Mathf.RoundToInt(costs * costMultiplyer); // Z�hle den Preis f�rs n�chste mal h�her
        myScoreSystem.looseWarningScreen.SetActive(false); // Spiel geht weiter
        myScoreSystem.ResetUnhappyGuestCount();

    }

    public void EsDraufAnkommenLassen()
    {
        myScoreSystem.looseWarningScreen.SetActive(false);
        myScoreSystem.LooseScreen.SetActive(true);

        myScoreSystem.loosUnhappyGuestCount = 10000;
    }

}
