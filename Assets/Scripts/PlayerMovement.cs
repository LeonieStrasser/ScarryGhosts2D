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
    // -- Selection
    Selection sl;
    bool selectionSwitcherTriggered = false;

   
    //--stairs
    bool stairsTriggered = false;
    [SerializeField]
    float stairsOffset = 2;

    public Rigidbody2D rb;

    private float horizontal;
    public float speed = 0f;
    private bool isFacingRight = true;          // <- das ist erst später für die Darstellung des Player-Sprite relevant

    public PlayerInput input;

    private InputAction moveInput;


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
        }
        else if (other.tag == "Stairs")
        {
            stairsTriggered = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        currentCollision = null;
        if (other.tag == "ModeSwitcher")
        {
            selectionSwitcherTriggered = false;
        }
        else if (other.tag == "Stairs")
        {
            stairsTriggered = false;
        }
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
        if (gm.IsPlayModeOn() == false)
        {
            sl.ChooseInputCheck();
        }
    }

    public void Interaction(InputAction.CallbackContext context) // auf X
    {
        if (selectionSwitcherTriggered)
        {
            gm.ChangeGameMode();
        }
        else if (stairsTriggered)
        {
            // nimm dir die treppe und schalte ihre collider an
            currentCollision.GetComponent<Stairs>().SwitchColliderState();
            // setze den Player auf das Podest
            transform.position = new Vector3(transform.position.x, transform.position.y + stairsOffset, transform.position.z);
        }

    }

    public void Back(InputAction.CallbackContext context)
    {
        Debug.Log("back");
        if(camChanger.IsHotelTrue())
        {
            camChanger.SetPlayerCam();
        }
    }

    public void HotelOverview(InputAction.CallbackContext context)
    {
        camChanger.SetHotelCam();

    }
    #endregion
}
