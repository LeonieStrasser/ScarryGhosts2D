using System.Collections;
using System.Collections.Generic;
using UnityEngine;










public class CharacterControllerRB : MonoBehaviour
{
    CapsuleCollider2D MyCollider;
    Rigidbody2D MyRigidbody;

    public bool Grounded = false;

    public bool DoubleJumpUsed = false;


    // Start is called before the first frame update
    void Start()
    {
        MyCollider = GetComponent<CapsuleCollider2D>();
        MyRigidbody = GetComponent<Rigidbody2D>();
        
    }

    void update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && Grounded == true || DoubleJumpUsed == false)
        {
            MyRigidbody.AddForce(new Vector2(0, 10), ForceMode2D.Impulse);
            if(Grounded == false)
            {
                DoubleJumpUsed = true;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        RaycastHit2D capsuleHit = Physics2D.CapsuleCast(MyCollider.bounds.center, MyCollider.bounds.size, CapsuleDirection2D.Vertical, 0, Vector2.down, 0.01f, ~ LayerMask.GetMask("Player"));

        Grounded = false;

        if(capsuleHit.collider != null)
        {
            Grounded = true;
            DoubleJumpUsed = false;
        }

        //Debug.Log("capsuleHit collider.name);


        if(Input.GetKey(KeyCode.D))
        {
            if(Input.GetKey(KeyCode.LeftShift))
            {
                MyRigidbody.AddForce(new Vector2(55, 0));
            }
            else
            {
            MyRigidbody.AddForce(new Vector2(5, MyRigidbody.velocity.y));
            }
        }
        
        else if(Input.GetKey(KeyCode.A))
        {
            if(Input.GetKey(KeyCode.LeftShift))
            {
                MyRigidbody.AddForce(new Vector2(-55, 0));
            }
            else
            {
            MyRigidbody.AddForce(new Vector2(-5, MyRigidbody.velocity.y));
            }
        }
        else
        {
            MyRigidbody.velocity /= new Vector2(2, 1);
        }
        
        
        
    }
}
