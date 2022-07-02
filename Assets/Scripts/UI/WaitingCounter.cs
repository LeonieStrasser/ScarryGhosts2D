using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaitingCounter : MonoBehaviour
{
    GameManager gm;
    public TextMeshProUGUI text;
    public Sprite waitingIcon;
    public Sprite warnIcon;
    public Sprite alarmIcon;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameIsRunning)
        {
            text.text = gm.waitingNPCs.Count.ToString();
        }
    }
}
