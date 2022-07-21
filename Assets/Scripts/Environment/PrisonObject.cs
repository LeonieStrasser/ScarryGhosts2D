using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PrisonObject : MonoBehaviour
{
    Interactable myInteractable;

    [SerializeField]
    GameObject ghostPrefab;
    [SerializeField]
    int minGhostRespawnTime;
    [SerializeField]
    int maxGhostRespawnTime;

    [SerializeField]
    GameObject prisonFullSprite;

    [SerializeField]
    TextMeshPro hintText;
    public float hintTextTime = 2;
    public string noGhostsText;
    public string fullText;

    //[SerializeField]
    //GameObject mainPrison;

    [HideInInspector]
    public int ghostsInPrison;


    //VFX
    public ParticleSystem[] emptyEffects;
    [SerializeField] GameObject spawnVFX;

    // Start is called before the first frame update
    void Awake()
    {
        myInteractable = GetComponent<Interactable>();
    }



    public void FillPrison(int ghostsToFill)
    {
        if (ghostsToFill > 0)
        {
            ghostsInPrison += ghostsToFill;
            foreach (var item in emptyEffects)
            {
                item.Play();
            }


            for (int i = 0; i < ghostsToFill; i++)
            {
                StartCoroutine(BreakeoutTimer());
            }

            myInteractable.IsActive = false;

            //Graphic
            prisonFullSprite.SetActive(true);

        }

    }

    IEnumerator BreakeoutTimer()
    {
        int timeUntillBreakeout = Random.Range(minGhostRespawnTime, maxGhostRespawnTime);

        yield return new WaitForSeconds(timeUntillBreakeout);

        ghostsInPrison--;
        if (ghostsInPrison < 0)
            ghostsInPrison = 0;

        if (ghostsInPrison == 0)
        {
            myInteractable.IsActive = true;
            //Graphic
            prisonFullSprite.SetActive(false);
        }

        GameObject breakeoutGhost = Instantiate(ghostPrefab, this.transform.position, Quaternion.identity);
        breakeoutGhost.GetComponent<NPC_Movement>().SetNewStartpoint(GetPrisonWaypoint()); // Spawne einen GEist und gebe seinem Movement den CHild Waypoint als Startort
        breakeoutGhost.GetComponent<Ghost>().BreakeOut();

        GameObject vfx = Instantiate(spawnVFX, this.transform.position, Quaternion.identity);
        vfx.SetActive(true);
    }

    public Waypoint GetPrisonWaypoint()
    {
        return this.GetComponentInChildren<Waypoint>();
    }

    public void SetIsFullText()
    {
        hintText.text = fullText;
        hintText.gameObject.SetActive(true);
        StartCoroutine(hintTextTimer());
    }

    public void SetNoGhostsText()
    {
        hintText.text = noGhostsText;
        hintText.gameObject.SetActive(true);
        StartCoroutine(hintTextTimer());
    }

    IEnumerator hintTextTimer()
    {
        yield return new WaitForSeconds(hintTextTime);
        hintText.gameObject.SetActive(false);
    }
}
