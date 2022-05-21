using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selection : MonoBehaviour
{
    public GameManager gm;
    // Selection States
    enum selectionState { npcSelection, roomSelection }
    selectionState currentState = selectionState.npcSelection;


    // Camera
    public float zoomInPlayMode = 3;
    public float zoomInSelectionMode = 5;
    public Vector2 cameraPositionWHileSelection;


    // NPC Selection
    GameObject selectedNPC;
    int selectionIndexNPC;

    // Room Selection
    GameObject selectedRoom;
    int selectionIndexRooms;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        switch (currentState)
        {
            case selectionState.npcSelection:
                // Navigation

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {

                    selectionIndexNPC++;
                    if (gm.waitingNPCs.Count > selectionIndexNPC)
                    {
                        LowlighDeselectedNPC();
                        selectedNPC = gm.waitingNPCs[selectionIndexNPC];
                        HighlightSelectedNPC();
                    }
                    else
                    {
                        selectionIndexNPC--;
                    }

                }

                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {

                    selectionIndexNPC--;
                    if (gm.waitingNPCs.Count > selectionIndexNPC && selectionIndexNPC >= 0)
                    {
                        LowlighDeselectedNPC();
                        selectedNPC = gm.waitingNPCs[selectionIndexNPC];
                        HighlightSelectedNPC();
                    }
                    else
                    {
                        selectionIndexNPC = 0;
                    }

                }

                // Sleection
                if (Input.GetKeyDown(KeyCode.Space) && selectedNPC != null)
                {
                    currentState = selectionState.roomSelection;

                    HighlightSelectedRoom();
                    //----------------Nur zum Testen ---------------Muss noch in schön gemacht werden
                    Camera.main.orthographicSize = zoomInSelectionMode;
                    Camera.main.transform.position = new Vector3(cameraPositionWHileSelection.x, cameraPositionWHileSelection.y, -10f);
                    //------------------------------------------------------------
                }

                break;
            case selectionState.roomSelection:

                // Navigation

                // Selection
                if(Input.GetKeyDown(KeyCode.Space) && selectedRoom != null)
                {
                    selectedNPC.GetComponent<Gast>().SetNewRoom(selectedRoom);
                }

                break;
            default:
                break;
        }

    }

    private void OnEnable()
    {
        currentState = selectionState.npcSelection;



        // SETZE DEN ERSTEN NPC DER WARTELISTE - SO EINER IN DER LOBBY WARTET
        selectionIndexNPC = 0;
        if (gm.waitingNPCs.Count > 0)
        {
            selectedNPC = gm.waitingNPCs[selectionIndexNPC];
            HighlightSelectedNPC();
        }
        else
        {
            selectedNPC = null; // Sollte die Lobby wieder leer sein, kann man nicht trotzdem in die Door Selection wechseln.
        }

        // SETZE DIE ERSTE TÜR DER FREIEN TÜRLISTE SO EINE FREI IST

        selectionIndexRooms = 0;
        if (gm.freeRooms.Count > 0)
        {
            selectedRoom = gm.freeRooms[0];
        }
        else
        {
            selectedRoom = null;
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
            selectedNPC = gm.waitingNPCs[selectionIndexNPC];
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

    void HighlightSelectedRoom()
    {
        selectedRoom.GetComponent<Room>().HighlightDoor();
    }
}
