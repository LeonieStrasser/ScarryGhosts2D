using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBackpack : MonoBehaviour
{
    public int ghostCount = 0;
    [SerializeField]
    int ghostLimit = 3;

    public GameObject[] ghostCountObjects;

    // Start is called before the first frame update
    void Start()
    {
        if (ghostCountObjects.Length != ghostLimit)
        {
            Debug.LogWarning("Jo! Es müssen so viele ghostCount Objekte am Backpack sein wie das ghostLimit! Du Nuss!");
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
}
