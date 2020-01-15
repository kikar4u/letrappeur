using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BreathingSystem))]
public class BreathingSystemEditor : Editor
{
    SerializedProperty canWalkDuringBreathing;
    private void OnEnable()
    {
        canWalkDuringBreathing = serializedObject.FindProperty("canWalkDuringBreathing");
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        BreathingSystem breathingSystem = (BreathingSystem)target;

        if (canWalkDuringBreathing.boolValue)
        {
            breathingSystem.walkSpeedDuringBreathing = EditorGUILayout.FloatField("Walkspeed during breathing", breathingSystem.walkSpeedDuringBreathing);
        }
    }
}
