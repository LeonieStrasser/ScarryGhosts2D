using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HUD_Manager : MonoBehaviour
{
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

    [Space(10)]

    [Header("Other UI")]
    [SerializeField]
    GameObject backTooltipUI;



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
}
