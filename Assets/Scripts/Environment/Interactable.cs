using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    [SerializeField]
    GameObject highlight;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
            highlight.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
            highlight.SetActive(false);
    }
}
