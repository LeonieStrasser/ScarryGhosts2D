using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    GameManager gm;
    ChangeCamera camChanger;
    [SerializeField]
    GameObject playerSprite;

    // Interaction
    GameObject currentCollision;
    [SerializeField]
    GameObject interactionUI;
    // -- Selection
    Selection sl;
    bool selectionSwitcherTriggered = false;


    //--stairs
    bool stairsTriggered = false;
    [SerializeField]
    float stairsOffset = 2;
    Stairs currentStairs;

    // Movement
    public Rigidbody2D rb;
    [SerializeField]
    SpriteRenderer mySprite;
    private float horizontal;
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

    // Weapon
    [SerializeField]
    GameObject beam;
    LineRenderer beamLine;
    enum weaponState { active, inactive }
    weaponState gunState = weaponState.inactive;
    Vector2 raycastDirection;
    [SerializeField]
    float beamRange = 5;

    public LayerMask ghostLayermask;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        sl = FindObjectOfType<Selection>();
        camChanger = FindObjectOfType<ChangeCamera>();
        beamLine = beam.GetComponent<LineRenderer>();
    }
    void Update()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);


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
        if (gunState == weaponState.active)
        {
            
            RaycastHit2D hit = Physics2D.Raycast(transform.position, raycastDirection, beamRange, ghostLayermask);
            beamLine.SetPosition(0, Vector3.zero); //startpunkt des Beams setzen
            Vector2 beamEnd = raycastDirection * beamRange;
            beamLine.SetPosition(1, beamEnd); //Endpunkt des Beams setzen

            if(hit.collider != null) // Wenn ein geist detected wurde muss er gefangen werden
            {
                if(hit.collider.gameObject.CompareTag("Ghost")) // Sicher gehen dass es auch wiiirklich ein Geist ist
                {
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, raycastDirection*beamRange);
    }

    void Flip()                         // <- das ist erst später für die Darstellung des Player-Sprite relevant, dürfte aber so übernommen werden können
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = playerSprite.transform.localScale;
        localScale.x *= -1;
        playerSprite.transform.localScale = localScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        currentCollision = other.gameObject;
        if (other.tag == "ModeSwitcher") // Wenn der Modeswitcher getriggert wurde, also der player am Lobbyobjekt steht, kann der selection mode gestartet werden.
        {
            selectionSwitcherTriggered = true;
            SetInteractionButton(true); // UI überm Player wird eingeschaltet
        }
        else if (other.tag == "Stairs")
        {
            stairsTriggered = true;
            SetInteractionButton(true); // UI überm Player wird eingeschaltet
        }


    }
    private void OnTriggerExit2D(Collider2D other)
    {
        currentCollision = null;
        if (other.tag == "ModeSwitcher")
        {
            selectionSwitcherTriggered = false;
            SetInteractionButton(false);
        }
        else if (other.tag == "Stairs")
        {
            stairsTriggered = false;
            SetInteractionButton(false);
        }


    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Wenn der Player von der Treppe auf den Boden wechselt muss die Treppe ausgeschaltet werden.
        if (other.gameObject.tag == "Ground" && currentStairs)
        {
            currentStairs.SetColliderInactive();
            currentStairs = null;
            stairsTriggered = false;
            // Bringe den Player auf die richtige Layer-Ebene
            mySprite.sortingOrder = gm.playerFlurLayer;
        }

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

    #region playerInput
    public void Move(InputAction.CallbackContext context)
    {

        if (gm.IsPlayModeOn() == true && camChanger.IsHotelTrue() == false) // Nur wenn der Selectionmode aus ist wird der Player bewegt
        {
            horizontal = context.ReadValue<Vector2>().x;            // <- movement, links, rechts

            // Beam Raycast wird in die Moving Direction getreht
            if(horizontal > 0)
            {
                raycastDirection = Vector2.right;
            }
            if (horizontal < 0)
            {
                raycastDirection = Vector2.left;
            }
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
            if (selectionSwitcherTriggered && gm.IsPlayModeOn() == true)
            {
                gm.ChangeGameMode();
            }
            else if (stairsTriggered && grounded)
            {
                // nimm dir die treppe und schalte ihre collider an
                currentStairs = currentCollision.GetComponent<Stairs>();
                currentStairs.SwitchColliderState();
                // setze den Player auf das Podest oder auf die Up-Position - jenachdem ob er unter dem Treppenzentrum ist, oder drüber
                if (transform.position.y < currentStairs.transform.position.y) // wenn player am Fuß der Treppe ist
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + stairsOffset, transform.position.z);
                }
                else // Wenn Player oben an der Treppe ist
                {
                    transform.position = new Vector3(currentStairs.upperEntrancePoint.position.x, transform.position.y, transform.position.z);
                }
                // Bringe den Player auf die richtige Layer-Ebene
                mySprite.sortingOrder = gm.treppenLayer - 1;
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
            if (camChanger.IsHotelTrue())
            {
                camChanger.SetPlayerCam();
            }
            if (gm.IsPlayModeOn() == false) // Wenn grade Selection Mode ist
            {
                gm.ChangeGameMode();
            }
        }
    }

    public void HotelOverview(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            camChanger.SetHotelCam();
        }
    }

    public void GhostMagnet(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Strahl einschalten
            beam.SetActive(true);
            gunState = weaponState.active;
        }
        if (context.canceled)
        {
            // Strahl ausschalten
            beam.SetActive(false);
            gunState = weaponState.inactive;
        }
    }
    #endregion
}
