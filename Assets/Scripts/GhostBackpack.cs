using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostBackpack : MonoBehaviour
{
    AudioScript audioManager;

    public int ghostCount = 0;
    [SerializeField]
    int ghostLimit = 3;

    public GameObject[] ghostCountObjects;
    public Animator anim;

    [Space(10)]
    [Header("ScareCounter")]
    [SerializeField]
    float Counter = 0;
    [SerializeField]
    float maxTimeUntillScare;
    [Tooltip("Wenn ein Neuer Geist eingesaugt wird, ist im Counter nur noch dieser Prozentsatz des aktuellen Counters �brig")]
    [SerializeField]
    float TimerReducePerNewGhost;
    [SerializeField]
    float warningTime;
    bool warningStarted = false;
    bool CounterIsRunning = false;

    [Space(5)]
    public GameObject scareTrigger;
    public ParticleSystem[] scareWarnVFX;
    public ParticleSystem[] scareVFX;

    [Header("Go throug walls power")]
    PlayerMovement myPlayer;
    GameObject[] allWalls;
    public Color transparentWallColor;
    Color wallColor;

    //Animation
    int zeroGhostIndex;
    int oneGhostIndex;
    int twoGhostIndex;
    int threeGhostIndex;

    // ScareEffect

    public Color scareColor;
    [SerializeField]
    SpriteRenderer playerSprite;

    private void Awake()
    {
        myPlayer = FindObjectOfType<PlayerMovement>();
        audioManager = FindObjectOfType<AudioScript>();
        zeroGhostIndex = anim.GetLayerIndex("ZeroGhost");
        oneGhostIndex = anim.GetLayerIndex("OneGhost");
        twoGhostIndex = anim.GetLayerIndex("TwoGhosts");
        threeGhostIndex = anim.GetLayerIndex("ThreeGhosts");
    }

    // Start is called before the first frame update
    void Start()
    {
        if (ghostCountObjects.Length != ghostLimit)
        {
            Debug.LogWarning("Jo! Es m�ssen so viele ghostCount Objekte am Backpack sein wie das ghostLimit! Du Nuss!");
        }

        Counter = maxTimeUntillScare;

        // Wall Power
        List<GameObject> goFindWalls = new List<GameObject>(GameObject.FindGameObjectsWithTag("Wall"));
        allWalls = goFindWalls.ToArray();

        wallColor = allWalls[0].GetComponentInChildren<SpriteRenderer>().color;
    }


    private void Update()
    {
        if (GameManager.Instance.gameIsRunning)
        {
            if (CounterIsRunning)
            {
                UpdateScareCounter();
            }
        }
    }

    void UpdateGhostCountUI()
    {
        for (int i = 0; i < ghostCountObjects.Length; i++)
        {
            ghostCountObjects[i].SetActive(false);
        }
        for (int i = 0; i < ghostCount; i++)
        {
            ghostCountObjects[i].SetActive(true);
        }

        //AnimationLayers

        switch (ghostCount)
        {
            case 0:
                anim.SetLayerWeight(zeroGhostIndex, 1);
                anim.SetLayerWeight(oneGhostIndex, 0);
                anim.SetLayerWeight(twoGhostIndex, 0);
                anim.SetLayerWeight(threeGhostIndex, 0);
                break;
            case 1:
                anim.SetLayerWeight(zeroGhostIndex, 0);
                anim.SetLayerWeight(oneGhostIndex, 1);
                anim.SetLayerWeight(twoGhostIndex, 0);
                anim.SetLayerWeight(threeGhostIndex, 0);
                break;
            case 2:
                anim.SetLayerWeight(zeroGhostIndex, 0);
                anim.SetLayerWeight(oneGhostIndex, 0);
                anim.SetLayerWeight(twoGhostIndex, 1);
                anim.SetLayerWeight(threeGhostIndex, 0);
                break;
            case 3:
                anim.SetLayerWeight(zeroGhostIndex, 0);
                anim.SetLayerWeight(oneGhostIndex, 0);
                anim.SetLayerWeight(twoGhostIndex, 0);
                anim.SetLayerWeight(threeGhostIndex, 1);
                break;
            default:
                break;
        }
    }

    public void AddGhost()
    {
        if (ghostCount < ghostLimit)
        {
            ghostCount++;
            UpdateGhostCountUI();


            // Wenn mehr als ein Geist gefangen ist, reduziere den Counter
            if (ghostCount > 1)
            {
                Counter *= TimerReducePerNewGhost;
            }
            // wenn der timer noch nicht l�uft setze ihn auf laufend und so
            if (Counter == maxTimeUntillScare)
            {
                CounterIsRunning = true;
            }

            if (ghostCount == ghostLimit && myPlayer.canGoThroughWalls)
            {
                for (int i = 0; i < allWalls.Length; i++)
                {
                    allWalls[i].GetComponentInChildren<Collider2D>().isTrigger = true;
                    // ColorTransparency
                    allWalls[i].GetComponentInChildren<SpriteRenderer>().color = transparentWallColor;
                }
            }
        }
    }

    public bool CheckForFreeSlots()
    {
        if (ghostCount < ghostLimit)
        {
            return true;
        }
        else
            return false;
    }

    public void EmptyOutBackpack(out int ghostCountOut)
    {
        ghostCountOut = ghostCount;
        ghostCount = 0;
        UpdateGhostCountUI();

        // Alle W�nde wieder undurchdringlich machen
        for (int i = 0; i < allWalls.Length; i++)
        {
            allWalls[i].GetComponentInChildren<Collider2D>().isTrigger = false;

            allWalls[i].GetComponentInChildren<SpriteRenderer>().color = wallColor;
        }

        //Audio Resetten
        audioManager.Stop("ScarryBackpack");
        audioManager.Stop("BackpackScareWarning");

        //Material
        playerSprite.color = Color.white;
    }

    // Wenn die Geister eine Gewisse Zeit im Rucksack sind, fangen sie irgendwann an, zu poltern
    void UpdateScareCounter()
    {
        Counter -= 1 * Time.deltaTime;

        if (Counter <= warningTime && !warningStarted)
        {
            warningStarted = true;

            foreach (var item in scareWarnVFX)
            {
                item.Play();
            }

            //Audio
            audioManager.Play("BackpackScareWarning");
        }

        if (Counter <= 0) // Player wird Scarry
        {
            OnBackpackGetsScarry();
            Counter = maxTimeUntillScare;
            CounterIsRunning = false;

            //Material
            playerSprite.color = scareColor;
        }


    }

    void OnBackpackGetsScarry()
    {
        Debug.Log("Im Scarry now!");

        foreach (var item in scareWarnVFX)
        {
            item.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }
        foreach (var item in scareVFX)
        {
            item.Play();
        }
        scareTrigger.SetActive(true);

        // AUDIO
        audioManager.Play("ScarryBackpack");
        audioManager.Stop("BackpackScareWarning");
    }


    public void SetBackpackCalm()
    {
        foreach (var item in scareWarnVFX)
        {
            item.Stop(false, ParticleSystemStopBehavior.StopEmitting); 
        }
        foreach (var item in scareVFX)
        {
            item.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }
        scareTrigger.SetActive(false);
        warningStarted = false;
        CounterIsRunning = false;
        Counter = maxTimeUntillScare;

        audioManager.Stop("ScarryBackpack");
    }

    public void FlipMyParticles()
    {
        GetComponent<ParticleFlip>().FlipParticleSystems();
    }
}
