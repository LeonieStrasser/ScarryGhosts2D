using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soul : MonoBehaviour
{
    Waypoint myTarget;
    Pathfinder myPathfinder;
    bool targetReached = false;

    public float speed = 10f;
    public ParticleSystem soulVFX;
    public Collider2D myTriggerCollider;

    public GameObject ghostPrefab;
    public int spawnTime;

    //AUDIO
    AudioScript audioManager;
    Sound[] mySounds;

    private void Awake()
    {
        transform.SetParent(null);
        myPathfinder = FindObjectOfType<Pathfinder>();
        audioManager = FindObjectOfType<AudioScript>();
    }

    private void Start()
    {
        myTarget = myPathfinder.GetSoulWaypoint();
        myTriggerCollider.enabled = false;
        audioManager.Initialize3dSound(this.gameObject, out mySounds);
        audioManager.Play3dSoundAtMySource("SoulSpawn", mySounds);
        audioManager.Play3dSoundAtMySource("SoulIdle", mySounds);
    }

    private void Update()
    {
        if (GameManager.Instance.gameIsRunning)
        {
            if (!targetReached)
                Move();
        }
    }

    public void Move()
    {
        transform.position = Vector2.MoveTowards(this.transform.position, myTarget.transform.position, speed * Time.deltaTime);

        if (transform.position.x == myTarget.transform.position.x && transform.position.y == myTarget.transform.position.y) // Wenn die Seele am Ziel angekommen ist kann man sie vernichten und sie beginnt zu zählen.
        {
            targetReached = true;
            soulVFX.Play();
            myTriggerCollider.enabled = true;
            StartCoroutine(ghostSpawnTimer());
        }
    }

    public void DestroySoul()
    {
        Destroy(gameObject);
    }

    IEnumerator ghostSpawnTimer()
    {
        yield return new WaitForSeconds(spawnTime);
        NPC_Movement ghostMovement = Instantiate(ghostPrefab, this.transform.position, Quaternion.identity).GetComponent<NPC_Movement>();
        ghostMovement.SetNewStartpoint(myTarget);

        //AUDIO
        audioManager.Play3dSoundAtMySource("SoulIBecomesGhost", mySounds);

        Destroy(this.gameObject);
    }
}
