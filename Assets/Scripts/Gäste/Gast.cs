using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gast : MonoBehaviour
{

    GameManager gm;

    GameManager myRoom;
    
    // Start is called before the first frame update
    void Start()
    {
        gm = GetComponent<GameManager>();
    }

    public bool DoÍHaveARoom()
    {
        if(myRoom)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
