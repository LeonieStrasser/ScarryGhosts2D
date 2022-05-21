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

                    if (selectedRoom)
                        HighlightSelectedRoom();

                    //----------------Nur zum Testen ---------------Muss noch in schön gemacht werden
                    Camera.main.orthographicSize = zoomInSelectionMode;
                    Camera.main.transform.position = new Vector3(cameraPositionWHileSelection.x, cameraPositionWHileSelection.y, -10f);
                    //------------------------------------------------------------
                }

                break;
            case selectionState.roomSelection:

                // Navigation
                if (Input.GetKeyDown(KeyCode.RightArrow) && selectedRoom)
                {
                    Room rightRoom = selectedRoom.GetComponent<Room>().rightNeighbour;
                    bool foundNextRoom = false;

                    while (!foundNextRoom)
                    {
                        if (rightRoom != null && rightRoom.free) // Ist der nachbarraum vorhanden und frei?
                        {
                            foundNextRoom = true;
                            LowlightSelectedRoom();
                            selectedRoom = rightRoom.gameObject;
                        }
                        else if (rightRoom != null && !rightRoom.free) // Ist der Nachbarraum zwar vorhanden aber belegt?
                        {
                            rightRoom = rightRoom.rightNeighbour;
                        }
                        else // Ist der Nachbarraum nicht vorhanden?
                        {
                            foundNextRoom = true;
                        }
                    }

                    if (rightRoom != null)
                    {
                        LowlightSelectedRoom();
                        selectedRoom = rightRoom.gameObject;
                        HighlightSelectedRoom();
                    }
                }

                if (Input.GetKeyDown(KeyCode.LeftArrow) && selectedRoom)
                {
                    Room leftRoom = selectedRoom.GetComponent<Room>().leftNeighbour;
                    bool foundNextRoom = false;

                    while (!foundNextRoom)
                    {
                        if (leftRoom != null && leftRoom.free) // Ist der nachbarraum vorhanden und frei?
                        {
                            foundNextRoom = true;
                            LowlightSelectedRoom();
                            selectedRoom = leftRoom.gameObject;
                        }
                        else if (leftRoom != null && !leftRoom.free) // Ist der Nachbarraum zwar vorhanden aber belegt?
                        {
                            leftRoom = leftRoom.leftNeighbour;
                        }
                        else // Ist der Nachbarraum nicht vorhanden?
                        {
                            foundNextRoom = true;
                        }
                    }

                    if (leftRoom != null)
                    {
                        LowlightSelectedRoom();
                        selectedRoom = leftRoom.gameObject;
                        HighlightSelectedRoom();
                    }
                }

                if (Input.GetKeyDown(KeyCode.UpArrow) && selectedRoom)
                {
                    Room upRoom = selectedRoom.GetComponent<Room>().upNeighbour;
                    Room checkRoom = upRoom;

                    bool foundNextRoom = false;
                    bool leftSideChecked = false;

                    while (!foundNextRoom) // Suche nach oben und dann nach links
                    {


                        switch (leftSideChecked)
                        {
                            case false:
                                if (checkRoom != null && checkRoom.free) // Ist der nachbarraum vorhanden und frei?
                                {
                                    foundNextRoom = true;
                                    LowlightSelectedRoom();
                                    selectedRoom = checkRoom.gameObject;
                                }
                                else if (checkRoom != null && !checkRoom.free) // Ist der Nachbarraum zwar vorhanden aber belegt?
                                {
                                    checkRoom = checkRoom.leftNeighbour; // erstmal nach links suchen
                                }
                                else // Ist der linke Nachbarraum nicht vorhanden?
                                {
                                    leftSideChecked = true; // vom Oberen Nachbar (besetzt) aus gibt es links keinen freien raum
                                    checkRoom = upRoom; // nächster Check geht wieder vom ersten Up room aus
                                }

                                break;
                            case true:

                                if (checkRoom != null && checkRoom.free) // Ist der nachbarraum vorhanden und frei?
                                {
                                    foundNextRoom = true;
                                    LowlightSelectedRoom();
                                    selectedRoom = checkRoom.gameObject;
                                }
                                else if (checkRoom != null && !checkRoom.free) // Ist der Nachbarraum zwar vorhanden aber belegt?
                                {
                                    checkRoom = checkRoom.rightNeighbour; // erstmal nach rechts suchen
                                }
                                else // Ist der rechte Nachbarraum nicht vorhanden?
                                {
                                    foundNextRoom = true; // vom Oberen Nachbar (besetzt) aus gibt es rechten keinen freien raum
                                    checkRoom = upRoom; // nächster Check geht wieder vom ersten Up room aus
                                }

                                break;
                            default:
                                break;
                        }
                    }


                    if (checkRoom != null)
                    {
                        HighlightSelectedRoom();
                    }
                }

                if (Input.GetKeyDown(KeyCode.DownArrow) && selectedRoom)
                {
                    Room downRoom = selectedRoom.GetComponent<Room>().downNeighbour;
                    if (downRoom != null)
                    {
                        LowlightSelectedRoom();
                        selectedRoom = downRoom.gameObject;
                        HighlightSelectedRoom();
                    }
                }

                // Selection
                if (Input.GetKeyDown(KeyCode.Space) && selectedRoom != null)
                {
                    // Gib dem gewählten NPC seinen Raum
                    selectedNPC.GetComponent<Gast>().SetNewRoom(selectedRoom);
                    // Lösche ihn von der waiting List
                    gm.RemoveMeFromWaitingList(selectedNPC);
                    // Setze den Raum auf "besetzt"
                    selectedRoom.GetComponent<Room>().free = false;
                    LowlightSelectedRoom();

                    //Wieder in die NPC Selection wechseln
                    LowlighDeselectedNPC();

                    StartNpcSelection();
                    // ----------------selected npc muss null sein, sonst kann man wieder per space in die Roomselection obwohl man keinen neuen npc gewählt hat
                }

                break;
            default:
                break;
        }

    }

    private void OnEnable()
    {
        StartNpcSelection();
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

    void StartNpcSelection()
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

    void LowlightSelectedRoom()
    {
        selectedRoom.GetComponent<Room>().LowlightDoor();
    }
}
