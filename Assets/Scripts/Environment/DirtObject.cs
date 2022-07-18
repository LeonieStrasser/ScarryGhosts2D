using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtObject : MonoBehaviour
{
    ScoreSystem myScore;
    Collider2D myCollider;

    bool iAmDirty = false;

    // Start is called before the first frame update
    void Start()
    {
        myScore = FindObjectOfType<ScoreSystem>();
        myCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (myCollider != null && !iAmDirty)
        {
            myScore.BloodyFurniture++;
            iAmDirty = true;
            Destroy(myCollider);
        }
    }

    
}
