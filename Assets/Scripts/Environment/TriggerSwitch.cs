using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSwitch : MonoBehaviour
{
    PlayerMovement player;
    public Stairs myStairs;
    public float upwardsMinValue = 0.01f; // Der Input Wert ab dem die treppe angeschaltet wird

    public bool downStairs;

    private void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "PlayerHeart" && !myStairs.directionFlipped)
        {
            if (player.rb.velocity.x < 0 && downStairs) // unten an der Treppe
            {
                if (player.vertical > upwardsMinValue)
                {
                    myStairs.SetColliderActive();
                    player.LayerBehindStairs(true);
                }
                else
                {
                    myStairs.SetColliderInactive();
                    player.LayerBehindStairs(false);
                }
            }
            if (player.rb.velocity.x > 0 && !downStairs) // oben an der Treppe
            {
                if (player.vertical < -upwardsMinValue)
                {
                    myStairs.SetColliderActive();
                    player.LayerBehindStairs(true);
                }
                else
                {
                    myStairs.SetColliderInactive();
                    player.LayerBehindStairs(false);
                }
            }

        }
        if (other.tag == "PlayerHeart" && myStairs.directionFlipped) // Wenn die Treppe andersrum steht
        {
            if (player.rb.velocity.x > 0 && downStairs) // unten an der Treppe
            {
                if (player.vertical > upwardsMinValue)
                {
                    myStairs.SetColliderActive();
                    player.LayerBehindStairs(true);
                }
                else
                {
                    myStairs.SetColliderInactive();
                    player.LayerBehindStairs(false);
                }
            }
            if (player.rb.velocity.x < 0 && !downStairs) // oben an der Treppe
            {
                if (player.vertical < -upwardsMinValue)
                {
                    myStairs.SetColliderActive();
                    player.LayerBehindStairs(true);
                }
                else
                {
                    myStairs.SetColliderInactive();
                    player.LayerBehindStairs(false);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "PlayerHeart" && !myStairs.directionFlipped)
        {
            if (player.rb.velocity.x > 0 && downStairs) // unten an der Treppe
            {
                myStairs.SetColliderInactive();
                player.LayerBehindStairs(false);
            }
            if (player.rb.velocity.x < 0 && !downStairs) // oben an der Treppe
            {
                myStairs.SetColliderInactive();
                player.LayerBehindStairs(false);
            }
        }
        if (other.tag == "PlayerHeart" && myStairs.directionFlipped) // Wenn die Treppe andersrum steht
        {
            if (player.rb.velocity.x < 0 && downStairs) // unten an der Treppe
            {
                myStairs.SetColliderInactive();
                player.LayerBehindStairs(false);
            }
            if (player.rb.velocity.x > 0 && !downStairs) // oben an der Treppe
            {
                myStairs.SetColliderInactive();
                player.LayerBehindStairs(false);
            }
        }
    }

}

