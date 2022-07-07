using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPC_Movement))]
public class Ghost : MonoBehaviour
{
    // Movement
    [SerializeField]
    NPC_Movement myMovement;

    [Header("Animation")]
    [SerializeField]
    Animator anim;

    void Start()
    {
              
        GoToRandomTarget();
    }



    public void GoToRandomTarget()
    {
        myMovement.GoToNewTarget(myMovement.GetRandomWaypoint());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Guest")
        {
            anim.SetTrigger("shock");
        }
    }

    public void DestroyMe()
    {
        Destroy(gameObject);
    }
}
