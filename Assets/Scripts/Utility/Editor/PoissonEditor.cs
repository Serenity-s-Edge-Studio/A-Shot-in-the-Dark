using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(PoissonSpawner), true)]
public class PoissonEditor : Editor
{
    private PoissonSpawner spawner;
    private void OnEnable()
    {
        spawner = target as PoissonSpawner;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Compute"))
        {
            spawner.GeneratePositions();
        }
    }
}
