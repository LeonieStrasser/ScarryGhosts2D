using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LooseEvents : MonoBehaviour
{
    //---------- Loose Screen Behörde bestechen
    ScoreSystem myScoreSystem;
    int costs;
    bool firstTimePayed = false;
    public TextMeshProUGUI schmierButtonTMP;
    public string schmierButtonText;

    public float firstCostMultiplyer = 0.6f;
    public float costMultiplyer = 1.2f;

    private void Awake()
    {
        myScoreSystem = FindObjectOfType<ScoreSystem>();
    }

    public void CheckForFirstLoose()
    {
        if (!firstTimePayed) // Beim ersten Mal wird der Preis anhand des erwirtschafteten Geldes errechnet
        {
            costs = Mathf.RoundToInt(myScoreSystem.scoreAmount * firstCostMultiplyer) +1;
            Debug.Log("First Costs are " + costs);
            firstTimePayed = true;

            schmierButtonText = (schmierButtonText + costs + "$").ToString();
            schmierButtonTMP.text = schmierButtonText;
        }
    }
    public void BehördeSchmieren()
    {
        myScoreSystem.scoreAmount = myScoreSystem.scoreAmount - costs; // Zahle das Schmiergeld
        costs = Mathf.RoundToInt(costs * costMultiplyer); // Zähle den Preis fürs nächste mal höher
        myScoreSystem.looseScreen.SetActive(false); // Spiel geht weiter
        myScoreSystem.ResetUnhappyGuestCount();

    }
}
