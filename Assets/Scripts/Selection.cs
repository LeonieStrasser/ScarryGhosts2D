using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Selection : MonoBehaviour
{
    public GameManager gm;
    // Selection States
    enum selectionState { npcSelection, roomSelection }
    selectionState currentState = selectionState.npcSelection;
    // Selection Input
    enum selectionInput { none, right, left, up, down }
    selectionInput currentInput = selectionInput.none;
    bool freeForNewInput = true; // Wird nach einem Input auf false gesetzt und erst wieder true gemacht wennd er Button losgelassen wird. so verhindert man einen dauer Input
    public bool choose = false; // wird vom Player getriggert wenn der Choose Input kommt

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

                if (currentInput == selectionInput.right && freeForNewInput)
                {
                    freeForNewInput = false;
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

                if (currentInput == selectionInput.left && freeForNewInput)
                {
                    freeForNewInput = false;
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
                if (choose && selectedNPC != null)
                {
                    choose = false; // k�nnte durch den l�ngeren Input evtl sofort wieder getriggert werden

                    currentState = selectionState.roomSelection;
                    HighlightAllFreeRooms();

                    if (selectedRoom)
                        HighlightSelectedRoom();

                    OnRoomSelection();
                }

                break;
            case selectionState.roomSelection:

                // Navigation
                if (currentInput == selectionInput.right && freeForNewInput && selectedRoom)
                {
                    freeForNewInput = false;
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

                if (currentInput == selectionInput.left && freeForNewInput && selectedRoom)
                {
                    freeForNewInput = false;
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

                if (currentInput == selectionInput.up && freeForNewInput && selectedRoom)
                {
                    freeForNewInput = false;
                    Room upRoom = selectedRoom.GetComponent<Room>().upNeighbour;
                    if (!upRoom)
                    {
                        break;
                    }

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
                                    checkRoom = upRoom; // n�chster Check geht wieder vom ersten Up room aus
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
                                    checkRoom = upRoom;
                                    if (checkRoom.upNeighbour) // Wenn es noch einen h�heren gibt
                                    {
                                        leftSideChecked = false;
                                        upRoom = upRoom.upNeighbour; // n�chster Check geht nun vom n�chst h�heren aus vom ersten Up room aus
                                        checkRoom = upRoom;
                                    }
                                    else
                                    {
                                        foundNextRoom = true; // vom Oberen Nachbar (besetzt) aus gibt es  keinen freien raum auf allen Ebenen

                                    }
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

                if (currentInput == selectionInput.down && freeForNewInput && selectedRoom)
                {
                    freeForNewInput = false;
                    Room downRoom = selectedRoom.GetComponent<Room>().downNeighbour;
                    if (!downRoom)
                    {
                        break;
                    }

                    Room checkRoom = downRoom;

                    bool foundNextRoom = false;
                    bool leftSideChecked = false;

                    while (!foundNextRoom) // Suche nach unten und dann nach links
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
                                    leftSideChecked = true; // vom unteren Nachbar (besetzt) aus gibt es links keinen freien raum
                                    checkRoom = downRoom; // n�chster Check geht wieder vom ersten down room aus
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
                                    checkRoom = downRoom;
                                    if (checkRoom.downNeighbour) // Wenn es noch einen h�heren gibt
                                    {
                                        leftSideChecked = false;
                                        downRoom = downRoom.downNeighbour; // n�chster Check geht nun vom n�chst tieferen aus vom ersten Up room aus
                                        checkRoom = downRoom;
                                    }
                                    else
                                    {
                                        foundNextRoom = true; // vom Oberen Nachbar (besetzt) aus gibt es  keinen freien raum auf allen Ebenen

                                    }
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

                // Selection
                if (choose && selectedRoom != null)
                {

                    choose = false;
                    // Gib dem gew�hlten NPC seinen Raum
                    selectedNPC.GetComponent<Gast>().SetNewRoom(selectedRoom);
                    // L�sche ihn von der waiting List
                    gm.RemoveMeFromWaitingList(selectedNPC);
                    // Setze den Raum auf "besetzt"
                    selectedRoom.GetComponent<Room>().SetDorAsFree(false);
                    gm.UpdateFreeRooms();
                    LowlightSelectedRoom();
                    LowlightAllFreeRooms();

                    // Lasse den NPC loslaufen und seinen Wartepunkt verlassen
                    selectedNPC.GetComponent<Gast>().LeaveLobby(false);

                    //Wieder in die NPC Selection wechseln
                    LowlighDeselectedNPC();

                    StartNpcSelection();
                    // ----------------selected npc muss null sein, sonst kann man wieder per space in die Roomselection obwohl man keinen neuen npc gew�hlt hat
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
        OnLeavingSelectionMode();

        if (gm.waitingNPCs.Count > 0)
        {
            selectedNPC = gm.waitingNPCs[selectionIndexNPC];
            LowlighDeselectedNPC();
        }

        if (selectedRoom)
        {
            LowlightSelectedRoom();
        }

        selectedRoom = null;
        selectedNPC = null;

    }

    void StartNpcSelection()
    {
        OnNpcSelection();

        currentState = selectionState.npcSelection;


        // SETZE DEN ERSTEN NPC DER WARTELISTE - SO EINER IN DER LOBBY WARTET
        selectionIndexNPC = 0;
        UpdateNpcSelection();


        // SETZE DIE ERSTE T�R DER FREIEN T�RLISTE SO EINE FREI IST

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

    public void UpdateNpcSelection()
    {
        if (currentState == selectionState.npcSelection && this.enabled == true)
        {
            if (gm.waitingNPCs.Count == 1)
            {
                selectionIndexRooms = 0;
            }
            if (gm.waitingNPCs.Count > 0)
            {
                selectedNPC = gm.waitingNPCs[selectionIndexNPC];
                HighlightSelectedNPC();
            }
            else
            {
                selectedNPC = null; // Sollte die Lobby wieder leer sein, kann man nicht trotzdem in die Door Selection wechseln.
            }
        }

        gm.OrderWaitinglistByX();

    }

    void UpdateWaitingList()
    {

    }

    void HighlightAllFreeRooms()
    {
        for (int i = 0; i < gm.freeRooms.Count; i++)
        {
            gm.freeRooms[i].GetComponent<Room>().HighlightDoorAsFree(true);
        }
    }

    void LowlightAllFreeRooms()
    {
        for (int i = 0; i < gm.freeRooms.Count; i++)
        {
            gm.freeRooms[i].GetComponent<Room>().HighlightDoorAsFree(false);
        }
    }

    void HighlightSelectedNPC()
    {
        if (selectedNPC)
            selectedNPC.GetComponent<Gast>().selectionHover.SetActive(true);
    }

    void LowlighDeselectedNPC()
    {
        if (selectedNPC)
            selectedNPC.GetComponent<Gast>().selectionHover.SetActive(false);
    }

    void HighlightSelectedRoom()
    {
        selectedRoom.GetComponent<Room>().HighlightDoorAsHovered();
    }

    void LowlightSelectedRoom()
    {
        selectedRoom.GetComponent<Room>().LowlightDoor();
    }

    void OnNpcSelection()
    {
        Debug.Log(" On NPC Selection");

        //----------------Nur zum Testen ---------------Muss noch in sch�n gemacht werden
        Camera.main.orthographicSize = zoomInPlayMode - 1;
        Camera.main.transform.position = new Vector3(0, -3.5f, -10f); // Hier muss die Kamera nat�rlich am Player h�ngen.
        //------------------------------------------------------------
    }

    void OnRoomSelection()
    {
        //Das Gastscript bekommt die Info, dass der Gast selected wurde
        selectedNPC.GetComponent<Gast>().OnIAmSelected();

        // Alle freien R�ume werden gehighlighted

        //----------------Nur zum Testen ---------------Muss noch in sch�n gemacht werden
        Camera.main.orthographicSize = zoomInSelectionMode;
        Camera.main.transform.position = new Vector3(cameraPositionWHileSelection.x, cameraPositionWHileSelection.y, -10f);
        //------------------------------------------------------------
    }

    void OnLeavingSelectionMode()
    {
        Debug.Log("Leaving Selectionmode");

        //----------------Nur zum Testen ---------------Muss noch in sch�n gemacht werden
        Camera.main.orthographicSize = zoomInPlayMode;
        Camera.main.transform.position = new Vector3(0, -3.5f, -10f); // Hier muss die Kamera nat�rlich am Player h�ngen.
        //------------------------------------------------------------
    }

    public string GetSelectedNpcName()
    {
        if (selectedNPC)
            return selectedNPC.name;
        else
            return null;
    }

    public void SetSelectedNpcNull()
    {
        if (selectedNPC)
            LowlighDeselectedNPC();

        selectedNPC = null;
    }


    public void SelectionSwitchInput(Vector2 inputValue)
    {
        Vector2 right = new Vector2(1, 0);
        Vector2 left = new Vector2(-1, 0);
        Vector2 up = new Vector2(0, 1);
        Vector2 down = new Vector2(0, -1);



        if (inputValue == right)
        {
            Debug.Log("Right");
            currentInput = selectionInput.right;

        }
        else if (inputValue == left)
        {
            Debug.Log("left");
            currentInput = selectionInput.left;
        }
        else if (inputValue == up)
        {
            Debug.Log("up");
            currentInput = selectionInput.up;
        }
        else if (inputValue == down)
        {
            Debug.Log("down");
            currentInput = selectionInput.down;
        }
        else
        {
            Debug.Log("other Value");
            currentInput = selectionInput.none;
            freeForNewInput = true;
        }
    }

    public void SelectionInput(InputAction.CallbackContext context)
    {
        if (gm.IsPlayModeOn() != true)
        {
            SelectionSwitchInput(context.ReadValue<Vector2>());
        }
    }

   public void ChooseInputCheck() // Choose Input wird nur getriggert, wenn schon ein npc gew�hlt wurde oder ein raum
    {
        if((currentState == selectionState.npcSelection && selectedNPC) || (currentState == selectionState.roomSelection && selectedRoom))
        {
            choose = true;
        }

    }
}
