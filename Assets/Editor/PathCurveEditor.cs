using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathCurve))]
public class PathCurveEditor : Editor
{
    SerializedProperty waypointsCurve;

    private void OnEnable()
    {
        waypointsCurve = serializedObject.FindProperty("waypointCurves");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PathCurve pathCurve = (PathCurve)target;

        //Because a line can't be draw with only one point
        if (GUI.changed && waypointsCurve.arraySize < 2)
        {
            waypointsCurve.arraySize = 2;
            waypointsCurve.serializedObject.ApplyModifiedProperties();
        }
        //Maybe not necessary, but I like it dirty
        EditorUtility.SetDirty(pathCurve);
    }
}
