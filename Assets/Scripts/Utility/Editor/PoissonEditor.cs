using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(PoissonTest))]
public class PoissonEditor : Editor
{
    private PoissonTest spawner;
    private void OnEnable()
    {
        spawner = target as PoissonTest;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Compute"))
        {
            spawner.Compute();
        }
    }
}
