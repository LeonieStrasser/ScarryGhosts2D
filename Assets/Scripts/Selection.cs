using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selection : MonoBehaviour
{
    public GameManager gm;
    // Selection States
    enum selectionState { npcSelection, roomSelection}
    selectionState currentState = selectionState.npcSelection;


    // Camera
    public float zoomInPlayMode = 3;
    public float zoomInSelectionMode = 5;
    public Vector2 cameraPositionWHileSelection;


    // NPC Selection
    GameObject selectedNPC;
    int selectionIndex;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        switch (currentState)
        {
            case selectionState.npcSelection:

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {

            selectionIndex++;
            if (gm.waitingNPCs.Count > selectionIndex)
            {
                LowlighDeselectedNPC();
                selectedNPC = gm.waitingNPCs[selectionIndex];
                HighlightSelectedNPC();
            }else
            {
                selectionIndex--;
            }

        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {

            selectionIndex--;
            if (gm.waitingNPCs.Count > selectionIndex && selectionIndex >= 0)
            {
                LowlighDeselectedNPC();
                selectedNPC = gm.waitingNPCs[selectionIndex];
                HighlightSelectedNPC();
            }
            else
            {
                selectionIndex = 0;
            }

        }

        if (Input.GetKeyDown(KeyCode.Space))
                {
                    currentState = selectionState.roomSelection;


                    //----------------Nur zum Testen ---------------Muss noch in schön gemacht werden
                    Camera.main.orthographicSize = zoomInSelectionMode;
                    Camera.main.transform.position = new Vector3(cameraPositionWHileSelection.x, cameraPositionWHileSelection.y, -10f);
                    //------------------------------------------------------------
                }

                break;
            case selectionState.roomSelection:



                break;
            default:
                break;
        }

    }

    private void OnEnable()
    {
        currentState = selectionState.npcSelection;

        


        selectionIndex = 0;
        if (gm.waitingNPCs.Count > 0)
        {
            selectedNPC = gm.waitingNPCs[selectionIndex];
            HighlightSelectedNPC();
        }
    }

    private void OnDisable()
    {

        //----------------Nur zum Testen ---------------Muss noch in schön gemacht werden
        Camera.main.orthographicSize = zoomInPlayMode;
        Camera.main.transform.position = new Vector3(0, -3.5f, -10f); // Hier muss die Kamera natürlich am Player hängen.
        //------------------------------------------------------------
        if (gm.waitingNPCs.Count > 0)
        {
            selectedNPC = gm.waitingNPCs[selectionIndex];
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
