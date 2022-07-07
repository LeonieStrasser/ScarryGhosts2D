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
    [SerializeField]
    float moveSensibility = 0.5f;
    [SerializeField]
    float flipSensibility = 0.2f;
    [HideInInspector]
    public float vertical;
    public float speed = 0f;
    private bool isFacingRight = true;          // <- das ist erst später für die Darstellung des Player-Sprite relevant

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

    // Animation
    [SerializeField]
    Animator anim;
    enum animationState { idle, move, setGun, gunBeam, resetGun }
    [SerializeField]
    animationState animationStatemachine = animationState.idle;

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
            AnimationSwitch();

            if (animationStatemachine != animationState.setGun
                && animationStatemachine != animationState.resetGun
                && !anim.GetNextAnimatorStateInfo(0).IsName("Player_waffeEinstecken")
                && Mathf.Abs(horizontal) > moveSensibility) // Nur bewegen wenn der Player grad nicht in der Waffen einsteck anim ist
            {
                rb.velocity = new Vector2(horizontal * speed, rb.velocity.y); // Movement

            }
            else
            {
                rb.velocity = Vector2.zero;
            }


            if (!isFacingRight && horizontal > flipSensibility)
            {
                Flip();

            }
            else if (isFacingRight && horizontal < -flipSensibility)
            {
                Flip();
            }



            // Wenn der Player in die Luft fliegt wird er auf den Boden gesetzt
            if (!grounded && !stairGrounded)
            {
                rb.AddForce(Vector2.down * downForce, ForceMode2D.Force);
            }

            // Wenn die Waffe Aktiv ist, sendet sie Raycasts um nach Geistern zu detecten
            if (gunState == weaponState.active && anim.GetCurrentAnimatorStateInfo(0).IsName("Player_GhostBeam"))
            {
                if (!beam.activeSelf) // beam anschalten
                {
                    beam.SetActive(true);
                    audioManager.Play("Saugen"); // Audio
                }

                RaycastHit2D hit = Physics2D.Raycast(transform.position, raycastDirection, beamRange, ghostLayermask);
                beamLine.SetPosition(0, Vector3.zero); //startpunkt des Beams setzen
                Vector2 beamEnd = Vector2.right * beamRange;
                beamLine.SetPosition(1, beamEnd); //Endpunkt des Beams setzen

                Debug.DrawRay(transform.position, raycastDirection * beamRange, Color.white, 3);

                // Movement währenddessen ausschalten
                rb.velocity = Vector2.zero;

                if (hit.collider != null) // Wenn ein geist detected wurde muss er gefangen werden
                {
                    if (hit.collider.gameObject.CompareTag("Ghost") && myBackpack.CheckForFreeSlots()) // Sicher gehen dass es auch wiiirklich ein Geist ist
                    {
                        Instantiate(ghostDestroyVFX, hit.collider.transform.position, Quaternion.identity);
                        hit.collider.GetComponentInParent<Ghost>().DestroyMe();
                        myBackpack.AddGhost();
                        StartCoroutine(BeamCooldown());
                    }
                    else if (hit.collider.gameObject.CompareTag("Soul"))
                    {
                        hit.collider.gameObject.GetComponent<Soul>().DestroySoul();
                    }

                }
            }
        }

        // UGLY Animation FIX

        //if(Mathf.Abs(horizontal) < moveSensibility && anim.GetBool(4)) // Wenn der player still steht und aber rennt in der anim
        //{
        //    SetIdleAnimation();
        //}
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawRay(transform.position, raycastDirection * beamRange);
    //}

    void Flip()                         // <- das ist erst später für die Darstellung des Player-Sprite relevant, dürfte aber so übernommen werden können
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = playerSprite.transform.localScale;
        localScale.x *= -1;
        playerSprite.transform.localScale = localScale;


        // Beam Raycast wird in die Moving Direction getreht
        if (isFacingRight)
        {
            raycastDirection = Vector2.right;
        }
        else
        {
            raycastDirection = Vector2.left;
        }
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
            SetInteractionButton(true); // UI überm Player wird eingeschaltet
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
        if (other.gameObject.tag == "Ground")
        {

            grounded = false;
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


            if (Mathf.Abs(horizontal) > moveSensibility && animationStatemachine != animationState.setGun && animationStatemachine != animationState.resetGun)
            {

                animationStatemachine = animationState.move;
            }
        }
        if (context.canceled)
        {
            if (animationStatemachine != animationState.setGun && animationStatemachine != animationState.resetGun)
                animationStatemachine = animationState.idle;
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
    /// Wenn der Interaction Knopf gedrückt wird, bassiert je nach Trigger Tag etwas anderes
    /// </summary>
    /// <param name="context"></param>
    public void Interaction(InputAction.CallbackContext context) // auf X
    {
        if (context.started)
        {
            if (selectionSwitcherTriggered && gm.IsPlayModeOn() == true) // In den Selection Mode gehen
            {
                gm.ChangeGameMode();


                audioManager.Play("PlingPlaceholder"); // Audio Selection Mode an
            }

            else if (prisonIsTriggered && gm.IsPlayModeOn()) // Die Geister aus dem Rucksack werden ins Prison gefüllt
            {
                myBackpack.EmptyOutBackpack(out int backpackGhostCount);
                myBackpack.SetBackpackCalm();
                currentPrison.FillPrison(backpackGhostCount);


                audioManager.Play("PlingPlaceholder"); // Audio Backpack leeren

                // AnimationState
                anim.SetInteger("GhostCount", myBackpack.ghostCount);
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

                // SOllte man auf einer Treppe gewesen sein, müssen alle treppen disabled werden
                for (int i = 0; i < allStairs.Length; i++)
                {
                    allStairs[i].SetColliderInactive();
                }
                audioManager.Play("PlingPlaceholder"); // Audio Back klick
            }
        }
    }

    public void SpecialSkill(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (selectionSwitcherTriggered && gm.IsPlayModeOn() == true && canCalmDownGuests) // Am LobbyObjekt y drücken
            {
                for (int i = 0; i < gm.waitingNPCs.Count; i++)
                {
                    gm.waitingNPCs[i].GetComponent<Gast>().ResetWaitingTimer();
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
                // Aktiviere das UI für den Mode
                hudMan.EnableOverviewModeUI();


                audioManager.Play("PlingPlaceholder"); // Audio Hotel Overview on
                camChanger.SetHotelCam();

            }

            else if (camChanger.IsHotelTrue())
            {
                camChanger.SetPlayerCam();
                hudMan.DisableOverviewModeUI(); // für den fall das grade das Overview UI an war
            }
        }
    }

    public void GhostMagnet(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Strahl einschalten

            gunState = weaponState.active;

            // Movement währenddessen ausschalten
            rb.velocity = Vector2.zero;


            //Animation
            SetAttackAnimation();
            animationStatemachine = animationState.setGun;
        }
        if (context.canceled)
        {
            // Strahl ausschalten
            beam.SetActive(false);
            gunState = weaponState.inactive;

            //Animation
            SetAttackAnimationFalse();
            animationStatemachine = animationState.resetGun;
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


    #region animation

    void AnimationSwitch()
    {
        switch (animationStatemachine)
        {
            case animationState.idle:
                SetIdleAnimation();
                break;
            case animationState.move:
                SetWalkAnimation();
                break;
            case animationState.setGun:
                SetAttackAnimation();
                break;
            case animationState.gunBeam:
                break;
            case animationState.resetGun:
                SetIdleAnimation();
                break;
            default:
                break;
        }
    }

    public void SetStatemachineIdle()
    {
        animationStatemachine = animationState.idle;
    }
    void SetIdleAnimation()
    {
        anim.SetBool("walk", false);
        anim.SetBool("idle", true);
    }
    void SetWalkAnimation()
    {
        anim.SetBool("walk", true);
        anim.SetBool("idle", false);
    }

    void SetAttackAnimation()
    {
        anim.SetTrigger("startBeam");
        anim.SetBool("ghostAttack", true);
        anim.SetBool("walk", false);
        anim.SetBool("idle", true);
    }

    void SetAttackAnimationFalse()
    {
        anim.SetBool("ghostAttack", false);
    }
    #endregion
}
