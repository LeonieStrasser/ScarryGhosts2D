using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    GameManager gm;
    ChangeCamera camChanger;

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
    [SerializeField]
    Stairs currentStairs;

    public Rigidbody2D rb;

    private float horizontal;
    public float speed = 0f;
    private bool isFacingRight = true;          // <- das ist erst später für die Darstellung des Player-Sprite relevant

    public PlayerInput input;

    private InputAction moveInput;
    [SerializeField]
    bool grounded;


    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        sl = FindObjectOfType<Selection>();
        camChanger = FindObjectOfType<ChangeCamera>();
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
    }



    void Flip()                         // <- das ist erst später für die Darstellung des Player-Sprite relevant, dürfte aber so übernommen werden können
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
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

    }


    #region playerInput
    public void Move(InputAction.CallbackContext context)
    {

        if (gm.IsPlayModeOn() == true && camChanger.IsHotelTrue() == false) // Nur wenn der Selectionmode aus ist wird der Player bewegt
        {
            horizontal = context.ReadValue<Vector2>().x;            // <- movement, links, rechts
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

    public void Interaction(InputAction.CallbackContext context) // auf X
    {
        if (context.started)
        {
            if (selectionSwitcherTriggered)
            {
                gm.ChangeGameMode();
            }
            else if (stairsTriggered)
            {
                // nimm dir die treppe und schalte ihre collider an
                currentStairs = currentCollision.GetComponent<Stairs>();
                currentStairs.SwitchColliderState();
                // setze den Player auf das Podest oder auf die Up-Position - jenachdem ob er unter dem Treppenzentrum ist, oder drüber

                if (transform.position.y < currentStairs.transform.position.y) // wenn player am Fuß d Treppe ist
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + stairsOffset, transform.position.z);
                }
                else // Wenn Player oben an der Treppe ist
                {
                    transform.position = new Vector3(currentStairs.upperEntrancePoint.position.x, transform.position.y, transform.position.z);
                }
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
        }
    }

    public void HotelOverview(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            camChanger.SetHotelCam();
        }
    }
    #endregion
}
