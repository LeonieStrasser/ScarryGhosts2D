using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    GameManager gm;
    HUD_Manager hudMan;
    AudioScript audioManager;
    ChangeCamera camChanger;
    [SerializeField]
    GameObject playerSprite;
    [SerializeField]
    SpriteRenderer[] mySpriterenderers;

    // Interaction

    [SerializeField]
    GameObject interactionUI;
    [SerializeField]
    GameObject killInteractionUI;

    // Input
    PlayerInput playerInput;
    EventSystem myEventsSystem;

    // -- Selection
    Selection sl;
    bool selectionSwitcherTriggered = false;


    //--stairs
    Stairs[] allStairs;
    [SerializeField]
    float stairsOffset = 2;

    //--ghost Prison
    bool prisonIsTriggered = false;
    PrisonObject currentPrison;

    // Movement
    public Rigidbody2D rb;

    private float horizontal;
    [HideInInspector]
    public float vertical;
    public float speed = 0f;
    private bool isFacingRight = true;          // <- das ist erst sp�ter f�r die Darstellung des Player-Sprite relevant

    public PlayerInput input;

    [SerializeField]
    bool grounded;
    [SerializeField]
    bool stairGrounded;
    [Tooltip("Kraft die auf den Player wirkt, sollte er in die Luft katapultiert werden")]
    [SerializeField]
    float downForce = 5;

    //-SKILLS----------------------------------------------
    //BackToLobby
    public bool backToLobbyIsActivated = true;
    public Transform lobbySpawnPoint;

    //Lobby Skill
    bool canCalmDownGuests = true;

    //Wall Skill
    public bool canGoThroughWalls = true;

    // Kill Skill
    public bool canKillGuests = true;
    List<Gast> fleeingGuestsInTrigger;

    // Weapon
    public GhostBackpack myBackpack;
    [SerializeField]
    GameObject beam;
    LineRenderer beamLine;
    enum weaponState { active, inactive }
    weaponState gunState = weaponState.inactive;
    Vector2 raycastDirection;
    [SerializeField]
    float beamRange = 5;
    float beamCooldown = 2;
    bool beamPrepared = true;
    public LayerMask ghostLayermask;
    public GameObject ghostDestroyVFX;

    //------------------------------------------------------SKILLS END

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        hudMan = FindObjectOfType<HUD_Manager>();
        audioManager = FindObjectOfType<AudioScript>();
        sl = FindObjectOfType<Selection>();
        camChanger = FindObjectOfType<ChangeCamera>();
        beamLine = beam.GetComponent<LineRenderer>();
        playerInput = GetComponent<PlayerInput>();
        myEventsSystem = FindObjectOfType<EventSystem>();

        fleeingGuestsInTrigger = new List<Gast>();

        Stairs[] foundStairs = FindObjectsOfType<Stairs>();
        allStairs = foundStairs;


    }
    void Update()
    {
        if (GameManager.Instance.gameIsRunning)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y); // Movement


            if (!isFacingRight && horizontal > 0f)
            {
                Flip();
            }
            else if (isFacingRight && horizontal < 0f)
            {
                Flip();
            }




            // Wenn der Player in die Luft fliegt wird er auf den Boden gesetzt
            if (!grounded && !stairGrounded)
            {
                rb.AddForce(Vector2.down * downForce);
            }

            // Wenn die Waffe Aktiv ist, sendet sie Raycasts um nach Geistern zu detecten
            if (gunState == weaponState.active && beamPrepared)
            {

                RaycastHit2D hit = Physics2D.Raycast(transform.position, raycastDirection, beamRange, ghostLayermask);
                beamLine.SetPosition(0, Vector3.zero); //startpunkt des Beams setzen
                Vector2 beamEnd = raycastDirection * beamRange;
                beamLine.SetPosition(1, beamEnd); //Endpunkt des Beams setzen

                if (hit.collider != null) // Wenn ein geist detected wurde muss er gefangen werden
                {
                    if (hit.collider.gameObject.CompareTag("Ghost") && myBackpack.CheckForFreeSlots()) // Sicher gehen dass es auch wiiirklich ein Geist ist
                    {
                        Instantiate(ghostDestroyVFX, hit.collider.transform.position, Quaternion.identity);
                        Destroy(hit.collider.gameObject);
                        myBackpack.AddGhost();
                        StartCoroutine(BeamCooldown());

                        //AUDIO GEIST EINSAUGEN
                        audioManager.Play("GhostWirdEingesaugt");
                    }
                    else if (hit.collider.gameObject.CompareTag("Soul"))
                    {
                        hit.collider.gameObject.GetComponent<Soul>().DestroySoul();

                        //AUDIO SEELE EINSAUGEN
                        audioManager.Play("SeeleWirdZerst�rt");
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, raycastDirection * beamRange);
    }

    void Flip()                         // <- das ist erst sp�ter f�r die Darstellung des Player-Sprite relevant, d�rfte aber so �bernommen werden k�nnen
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = playerSprite.transform.localScale;
        localScale.x *= -1;
        playerSprite.transform.localScale = localScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "ModeSwitcher") // Wenn der Modeswitcher getriggert wurde, also der player am Lobbyobjekt steht, kann der selection mode gestartet werden.
        {
            selectionSwitcherTriggered = true;
        }
        else if (other.tag == "Stairs")
        {

        }
        else if (other.tag == "prisonObject")
        {
            prisonIsTriggered = true;
            currentPrison = other.GetComponent<PrisonObject>();
            SetInteractionButton(true); // UI �berm Player wird eingeschaltet
        }

        if(other.tag == "Wall")
        {
            //AUDIO
            audioManager.Play("GoThroughtWalls");
        }

        if (other.tag == "Guest")
        {
            Gast triggerGast = other.GetComponent<Gast>();
            if (triggerGast.IsGuestFleeing())
            {
                fleeingGuestsInTrigger.Add(triggerGast);
            }

            // Kill UI
            if (fleeingGuestsInTrigger.Count > 0)
            {
                killInteractionUI.SetActive(true);
            }
            else
            {
                killInteractionUI.SetActive(false);
            }
        }


    }
    private void OnTriggerExit2D(Collider2D other)
    {

        if (other.tag == "ModeSwitcher")
        {
            selectionSwitcherTriggered = false;
            SetInteractionButton(false);
        }
        else if (other.tag == "prisonObject")
        {
            prisonIsTriggered = false;
            SetInteractionButton(false);
        }
        if (other.tag == "Wall")
        {
            //AUDIO
            audioManager.Stop("GoThroughtWalls");
        }

        if (other.tag == "Guest")
        {
            Gast triggerGast = other.GetComponent<Gast>();

            if (fleeingGuestsInTrigger.Find(x => x == triggerGast))
                fleeingGuestsInTrigger.Remove(triggerGast);

            // Kill UI
            if (fleeingGuestsInTrigger.Count > 0)
            {
                killInteractionUI.SetActive(true);
            }
            else
            {
                killInteractionUI.SetActive(false);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {

    }
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Ground")
        {

            grounded = true;
        }
        else
            grounded = false;

        if (other.gameObject.tag == "Stairs")
        {
            stairGrounded = true;
        }
        else
            stairGrounded = false;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Stairs")
        {
            stairGrounded = false;
        }
    }

    void SetSortingOrder(int sortingLayer, SpriteRenderer[] sprites)
    {
        int baseLayerNr = sprites[0].sortingOrder;

        for (int i = 0; i < sprites.Length; i++)
        {
            int orderOffset = sprites[i].sortingOrder - baseLayerNr;
            sprites[i].sortingOrder = sortingLayer + orderOffset;
        }
    }

    public void LayerBehindStairs(bool state)
    {
        if (state == true)
        {
            SetSortingOrder(gm.playerBehindTreppe, mySpriterenderers);
        }
        if (state == false)
        {
            SetSortingOrder(gm.playerFlurLayer, mySpriterenderers);
        }
    }

    IEnumerator BeamCooldown()
    {
        beamPrepared = false;
        yield return new WaitForSeconds(beamCooldown);
        beamPrepared = true;
    }

    #region playerInput
    public void Move(InputAction.CallbackContext context)
    {

        if (gm.IsPlayModeOn() == true && camChanger.IsHotelTrue() == false) // Nur wenn der Selectionmode aus ist wird der Player bewegt
        {
            horizontal = context.ReadValue<Vector2>().x;            // <- movement, links, rechts

            // Beam Raycast wird in die Moving Direction getreht
            if (horizontal > 0)
            {
                raycastDirection = Vector2.right;
            }
            if (horizontal < 0)
            {
                raycastDirection = Vector2.left;
            }
        }

        // AUDIO MOVE
        if (context.started)
        {
            if (grounded)
            {
                audioManager.Play("PlayerStepOnGround");
                audioManager.Stop("PlayerStepOnStairs");

            }
            else if (stairGrounded)
            {
                audioManager.Play("PlayerStepOnStairs");
                audioManager.Stop("PlayerStepOnGround");
            }
        }
        else if (context.canceled)
        {
            audioManager.Stop("PlayerStepOnGround");
            audioManager.Stop("PlayerStepOnStairs");
        }
    }

    public void Choose(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (gm.IsPlayModeOn() == false)
            {
                sl.ChooseInputCheck();
            }
        }
    }

    /// <summary>
    /// Wenn der Interaction Knopf gedr�ckt wird, bassiert je nach Trigger Tag etwas anderes
    /// </summary>
    /// <param name="context"></param>
    public void Interaction(InputAction.CallbackContext context) // auf X
    {
        if (context.started)
        {
            if (selectionSwitcherTriggered && gm.IsPlayModeOn() == true) // In den Selection Mode gehen
            {
                gm.ChangeGameMode();


                audioManager.Play("SelectionModeOn"); // Audio Selection Mode an
            }

            else if (prisonIsTriggered && gm.IsPlayModeOn()) // Die Geister aus dem Rucksack werden ins Prison gef�llt
            {
                myBackpack.EmptyOutBackpack(out int backpackGhostCount);
                myBackpack.SetBackpackCalm();
                currentPrison.FillPrison(backpackGhostCount);


                audioManager.Play("ClearBackpack"); // Audio Backpack leeren
            }
            else if (gm.IsPlayModeOn() == false) // Wenn grade Selection Mode ist
            {
                gm.ChangeGameMode();
            }
        }
    }

    /// <summary>
    /// Wenn der Player ein Interactable triggert, wird ihm der Button angezeigt mit dem er es aktivieren kann.
    /// </summary>
    void SetInteractionButton(bool uiState)
    {
        interactionUI.SetActive(uiState);
    }

    public void Back(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (backToLobbyIsActivated && myBackpack.ghostCount > 0)
            {
                transform.position = lobbySpawnPoint.position;

                // SOllte man auf einer Treppe gewesen sein, m�ssen alle treppen disabled werden
                for (int i = 0; i < allStairs.Length; i++)
                {
                    allStairs[i].SetColliderInactive();
                }
                audioManager.Play("Teleport"); // Audio Back klick
            }
        }
    }

    public void SpecialSkill(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (selectionSwitcherTriggered && gm.IsPlayModeOn() == true && canCalmDownGuests) // Am LobbyObjekt y dr�cken
            {
                for (int i = 0; i < gm.waitingNPCs.Count; i++)
                {
                    gm.waitingNPCs[i].GetComponent<Gast>().ResetWaitingTimer();

                    // AUDIO
                    audioManager.Play("CalmDown");
                }
            }
        }
    }

    public void HotelOverview(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (gm.IsPlayModeOn() && !camChanger.IsHotelTrue())
            {
                // Aktiviere das UI f�r den Mode
                hudMan.EnableOverviewModeUI();


                audioManager.Play("HotelViewOn"); // Audio Hotel Overview on
                camChanger.SetHotelCam();
                               
            }

            else if (camChanger.IsHotelTrue())
            {
            }
        }

        if(context.canceled)
        {
                camChanger.SetPlayerCam();
                hudMan.DisableOverviewModeUI(); // f�r den fall das grade das Overview UI an war

                // AUDIO
                audioManager.Play("HotelViewOff");

        }
    }

    public void GhostMagnet(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Strahl einschalten
            beam.SetActive(true);
            gunState = weaponState.active;

            audioManager.Play("Saugen"); // Audio
        }
        if (context.canceled)
        {
            // Strahl ausschalten
            beam.SetActive(false);
            gunState = weaponState.inactive;

            audioManager.Stop("Saugen"); // Audio
        }
    }

    public void SetVerticalStairsInput(InputAction.CallbackContext context)
    {
        vertical = context.ReadValue<Vector2>().y;
    }


    public void Kill(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (killInteractionUI.activeSelf)
            {
                Gast[] opfer = fleeingGuestsInTrigger.ToArray(); // Speicher die Opfer zwischen
                fleeingGuestsInTrigger.Clear();
                for (int i = 0; i < opfer.Length; i++)
                {
                    opfer[i].Die();

                    // AUDIO
                    audioManager.Play("Kill");
                }
            }
        }
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            hudMan.PauseUIActive();
            GameManager.Instance.GamePause();
        }
    }


    public void SwitchActionMap(string mapName)
    {
        playerInput.SwitchCurrentActionMap(mapName);
    }

    public void SetFirstButton(Button firstButton)
    {
        myEventsSystem.firstSelectedGameObject = firstButton.gameObject;
    }
    #endregion
}
