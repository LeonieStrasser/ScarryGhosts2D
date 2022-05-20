using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selection : MonoBehaviour
{
    public GameManager gm;


    // NPC Selection
    GameObject selectedNPC;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        
        //----------------Nur zum Testen ---------------Muss noch in schön gemacht werden
        Camera.main.orthographicSize = 5;
        Camera.main.transform.position = new Vector3(0, 0, -10f);
        //------------------------------------------------------------

        if (gm.waitingNPCs.Count > 0)
        {
            selectedNPC = gm.waitingNPCs[0];
            HighlightSelectedNPC();
        }
    }

    private void OnDisable()
    {

        //----------------Nur zum Testen ---------------Muss noch in schön gemacht werden
        Camera.main.orthographicSize = 3;
        Camera.main.transform.position = new Vector3(0, -3.5f, -10f);
        //------------------------------------------------------------
        if (gm.waitingNPCs.Count > 0)
        {
            selectedNPC = gm.waitingNPCs[0];
            LowlighDeselectedNPC();
        }
    }

    void HighlightSelectedNPC()
    {
        selectedNPC.GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }

    void LowlighDeselectedNPC()
    {
        selectedNPC.GetComponentInChildren<SpriteRenderer>().color = Color.black;
    }
}
