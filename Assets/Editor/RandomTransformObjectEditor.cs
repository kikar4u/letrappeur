using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomTransformObject))]
public class RandomTransformObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        RandomTransformObject randomTransformObject = (RandomTransformObject)target;
        if (GUILayout.Button("Randomize Rotation"))
        {
            randomTransformObject.RandomizeRotation();
        }

        if (GUILayout.Button("Randomize Mesh"))
        {
            randomTransformObject.RandomizeMesh();
        }
        if (GUILayout.Button("Randomize Scale"))
        {
            randomTransformObject.RandomizeScale();
        }
    }

    private void OnValidate()
    {
        Debug.Log("OnValidate");
    }
}
