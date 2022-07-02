using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Spawner : MonoBehaviour
{
    [Tooltip("Hier das Prefab reinziehen, das gespawnt werden soll!")]
    public GameObject spawnPrefab;

    // Wave Data
    [Tooltip("Die mindestzeit, die zwischen zwei Spawns vergehen soll.")]
    public int minTime;
    [Tooltip("Die maximalzeit, die zwischen zwei Spawns vergehen soll.")]
    public int maxTime;
    public int waveSize;
    //-----------
    // Waves
    public GuestWave[] allWaves;

    int waveIndex = 0;

    int count = 0;

    int timeToNextSPawn;
    GameManager gm;

    bool npcSpawn;

    private void Start()
    {
        npcSpawn = false;
        gm = FindObjectOfType<GameManager>();
        SetRandomSpawntime();
        NextWave(waveIndex);
    }

    private void Update()
    {
        if (GameManager.Instance.gameIsRunning)
        {
            if (npcSpawn == false && waveSize > 0 && (gm.allWaitingPoints.Length > gm.waitingNPCs.Count))
            {
                npcSpawn = true;
                SetRandomSpawntime();
                StartCoroutine(SpawnerTime());
            }

            if (waveSize <= 0) NextWave(waveIndex);
        }
    }

    IEnumerator SpawnerTime()
    {
        yield return new WaitForSeconds(timeToNextSPawn);
        if (gm.allWaitingPoints.Length > gm.waitingNPCs.Count)
        {
            GameObject newNPC = Instantiate(spawnPrefab, this.transform.position, this.transform.rotation); // Spawne neuen NPC
            newNPC.name = "NPC " + count;
            count++;
            waveSize--;
        }
        npcSpawn = false;
    }

    void SetRandomSpawntime()
    {
        timeToNextSPawn = Random.Range(minTime, maxTime);
    }

    /// <summary>
    /// Überträgt die daten der nächsten Welle auf den NPC Spawner
    /// </summary>
    /// <param name="index"></param>
    void NextWave(int index)
    {
        if (waveIndex <= allWaves.Length - 1)
        {

            minTime = allWaves[index].minTime;
            maxTime = allWaves[index].maxTime;
            waveSize = allWaves[index].waveSize;

            if (waveIndex < allWaves.Length - 1) waveIndex++;
        }
    }
}
