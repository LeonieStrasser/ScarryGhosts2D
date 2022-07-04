using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonObject : MonoBehaviour
{
    public ParticleSystem emptyEffect;

    [SerializeField]
    GameObject ghostPrefab;
    [SerializeField]
    int minGhostRespawnTime;
    [SerializeField]
    int maxGhostRespawnTime;



    //[SerializeField]
    //GameObject mainPrison;


    int ghostsInPrison;

    // Start is called before the first frame update
    void Start()
    {

    }

    

    public void FillPrison(int ghostsToFill)
    {

        ghostsInPrison += ghostsToFill;
        emptyEffect.Play();

        for (int i = 0; i < ghostsToFill; i++)
        {
            StartCoroutine(BreakeoutTimer());
        }

    }

    IEnumerator BreakeoutTimer()
    {
        int timeUntillBreakeout = Random.Range(minGhostRespawnTime, maxGhostRespawnTime);

        yield return new WaitForSeconds(timeUntillBreakeout);

        ghostsInPrison--;
        if (ghostsInPrison < 0)
            ghostsInPrison = 0;

        GameObject breakeoutGhost = Instantiate(ghostPrefab, this.transform.position, Quaternion.identity);
        breakeoutGhost.GetComponent<NPC_Movement>().SetNewStartpoint(GetPrisonWaypoint()); // Spawne einen GEist und gebe seinem Movement den CHild Waypoint als Startort
        breakeoutGhost.GetComponent<Ghost>().BreakeOut();
    }

    public Waypoint GetPrisonWaypoint()
    {
        return this.GetComponentInChildren<Waypoint>();
    }
}
