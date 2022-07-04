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

    [Space(10)]
    [Header("ScareCounter")]
    [SerializeField]
    float Counter = 0;
    [SerializeField]
    float maxTimeUntillScare;
    [Tooltip("Wenn ein Neuer Geist eingesaugt wird, ist im Counter nur noch dieser Prozentsatz des aktuellen Counters übrig")]
    [SerializeField]
    float TimerReducePerNewGhost;
    [SerializeField]
    float warningTime;
    bool warningStarted = false;
    bool CounterIsRunning = false;

    [Space(5)]
    public GameObject scareTrigger;
    public ParticleSystem scareWarnVFX;
    public ParticleSystem scareFVX;

    [Header("Go throug walls power")]
    PlayerMovement myPlayer;
    GameObject[] allWalls;


    private void Awake()
    {
        myPlayer = FindObjectOfType<PlayerMovement>();
        audioManager = FindObjectOfType<AudioScript>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (ghostCountObjects.Length != ghostLimit)
        {
            Debug.LogWarning("Jo! Es müssen so viele ghostCount Objekte am Backpack sein wie das ghostLimit! Du Nuss!");
        }

        Counter = maxTimeUntillScare;

        // Wall Power
        List<GameObject> goFindWalls = new List<GameObject>(GameObject.FindGameObjectsWithTag("Wall"));
        allWalls = goFindWalls.ToArray();
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
            // wenn der timer noch nicht läuft setze ihn auf laufend und so
            if (Counter == maxTimeUntillScare)
            {
                CounterIsRunning = true;
            }

            if (ghostCount == ghostLimit && myPlayer.canGoThroughWalls)
            {
                for (int i = 0; i < allWalls.Length; i++)
                {
                    allWalls[i].GetComponentInChildren<Collider2D>().isTrigger = true;
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

        // Alle Wände wieder undurchdringlich machen
        for (int i = 0; i < allWalls.Length; i++)
        {
            allWalls[i].GetComponentInChildren<Collider2D>().isTrigger = false;
        }
    }

    // Wenn die Geister eine Gewisse Zeit im Rucksack sind, fangen sie irgendwann an, zu poltern
    void UpdateScareCounter()
    {
        Counter -= 1 * Time.deltaTime;

        if (Counter <= warningTime && !warningStarted)
        {
            warningStarted = true;
            scareWarnVFX.Play();
        }

        if (Counter <= 0) // Player wird Scarry
        {
            OnBackpackGetsScarry();
            Counter = maxTimeUntillScare;
            CounterIsRunning = false;
        }
    }

    void OnBackpackGetsScarry()
    {
        Debug.Log("Im Scarry now!");
        scareWarnVFX.Stop();
        scareFVX.Play();
        scareTrigger.SetActive(true);

        // AUDIO
        audioManager.Play("ScarryBackpack");
    }

    void OnBackpackWarning()
    {
        scareWarnVFX.Play();
    }

    public void SetBackpackCalm()
    {
        scareFVX.Stop();
        scareWarnVFX.Stop();
        scareTrigger.SetActive(false);
        warningStarted = false;
        CounterIsRunning = false;
        Counter = maxTimeUntillScare;

        audioManager.Stop("ScarryBackpack");
    }
}
