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

    //AUDIO
    AudioScript audioManager;
    Sound[] mySounds;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioScript>();
    }
    void Start()
    {
        mySounds = audioManager.Get3dSounds();
        audioManager.Initialize3dSound(this.gameObject, mySounds);

        GoToRandomTarget();

        //Audio
        audioManager.Play3dSoundAtMySource("GhostMoving", mySounds);
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
            audioManager.Play3dSoundAtMySource("GhostScare", mySounds);
        }
    }

    public void DestroyMe()
    {
        Destroy(gameObject);
    }
    
    public void BreakeOut()
    {
        audioManager.Play3dSoundAtMySource("GhostBreakout", mySounds);
    }
}
