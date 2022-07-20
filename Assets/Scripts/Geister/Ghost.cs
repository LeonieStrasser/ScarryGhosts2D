using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPC_Movement))]
public class Ghost : MonoBehaviour
{
    // STate
    bool breakeOut = false;
    bool soulSpawn = false;

    // Movement
    [SerializeField]
    NPC_Movement myMovement;

    [Header("Animation")]
    [SerializeField]
    Animator anim;

    //AUDIO
    AudioScript audioManager;
    Sound[] mySounds;
    [SerializeField]
    string[] breakeOutSounds;
    [SerializeField]
    string[] spawnSounds;

    // VFX
    
    [SerializeField]
    ParticleSystem dustVFX;
    [SerializeField]
    ParticleSystem localDustVFX;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioScript>();
    }
    void Start()
    {

        audioManager.Initialize3dSound(this.gameObject, out mySounds);

        GoToRandomTarget();

        //Audio
        audioManager.Play3dSoundAtMySource("GhostMoving", mySounds);

        if (breakeOut)
        {
            audioManager.PlayOneOfThese3DSounds(breakeOutSounds, mySounds);
            
        }
        else if (soulSpawn)
            audioManager.PlayOneOfThese3DSounds(spawnSounds, mySounds);
        {
            audioManager.PlayOneOfThese3DSounds(breakeOutSounds, mySounds);
           
        }

    }



    public void GoToRandomTarget()
    {
        myMovement.GoToNewTarget(myMovement.GetRandomWaypoint());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Guest")
        {
            anim.SetTrigger("shock");
            audioManager.Play3dSoundAtMySource("GhostScare", mySounds);
        }
    }

    public void DestroyMe()
    {
        

        dustVFX.transform.SetParent(null);
        dustVFX.Stop();
        Destroy(dustVFX.gameObject, 10);

        localDustVFX.transform.SetParent(null);
        localDustVFX.Stop();
        Destroy(localDustVFX.gameObject, 10);

        Destroy(gameObject);
    }

    public void BreakeOut()
    {
        breakeOut = true;
    }

    public void SpawnFromSoul()
    {
        soulSpawn = true;
    }
}
