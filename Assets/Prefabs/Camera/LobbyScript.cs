using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScript : MonoBehaviour
{
    public CinemachineSwitcher camSwitcher;
    void Start()
    {
        camSwitcher = FindObjectOfType<CinemachineSwitcher>();
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            camSwitcher.SwitchState();
            Debug.Log("Hello");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            camSwitcher.SwitchState();

        }
    }
    void Update()
    {
        
    }
}