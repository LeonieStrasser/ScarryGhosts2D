using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    [SerializeField]
    GameObject highlight;

    private bool isActive = true;
    public bool IsActive
    {
        get
        {
            return isActive;
        }
        set
        {
            isActive = value;
            if(value == false)
            {
                highlight.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && isActive)
            highlight.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player" && isActive)
            highlight.SetActive(false);
    }
}
