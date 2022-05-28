using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpawnDebug : MonoBehaviour
{
    public NPC_Movement prefab;
    public int index = 0;

    public void SpawnEntity ()
    {
        NPC_Movement vskh = Instantiate(prefab);
        vskh.id = index;
        index++;
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(SpawnDebug))]
public class SpawnDebugEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("Spawn NPC"))
        {
            ((SpawnDebug)target).SpawnEntity();
        }
        base.OnInspectorGUI();
    }
}

#endif
