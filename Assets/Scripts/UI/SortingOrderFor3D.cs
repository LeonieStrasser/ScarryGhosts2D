using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingOrderFor3D : MonoBehaviour
{
    public int sortingOrder = 0;

    private void Awake()
    {
        gameObject.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
    }
}
