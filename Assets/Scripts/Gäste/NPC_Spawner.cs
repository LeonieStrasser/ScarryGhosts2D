using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Spawner : MonoBehaviour
{
    [Tooltip("Hier das Prefab reinziehen, das gespawnt werden soll!")]
    public GameObject spawnPrefab;
    [Tooltip("Die mindestzeit, die zwischen zwei Spawns vergehen soll.")]
    public int minTime;
    [Tooltip("Die maximalzeit, die zwischen zwei Spawns vergehen soll.")]
    public int maxTime;
    int count = 0;

    int timeToNextSPawn;
    GameManager gm;

    bool npcSpawn;

    private void Start()
    {
        npcSpawn = false;
        gm = FindObjectOfType<GameManager>();
        SetRandomSpawntime();

    }

    private void Update()
    {
        if (GameManager.Instance.gameIsRunning)
        {
            if (npcSpawn == false && (gm.allWaitingPoints.Length > gm.waitingNPCs.Count))
            {
                npcSpawn = true;
                SetRandomSpawntime();
                StartCoroutine(SpawnerTime());
            }
        }
    }

    IEnumerator SpawnerTime()
    {
        yield return new WaitForSeconds(timeToNextSPawn);
        GameObject newNPC = Instantiate(spawnPrefab, this.transform.position, this.transform.rotation); // Spawne neuen NPC
        newNPC.name = "NPC " + count;
        count++;
        npcSpawn = false;
    }

    void SetRandomSpawntime()
    {
        timeToNextSPawn = Random.Range(minTime, maxTime);
    }
}
