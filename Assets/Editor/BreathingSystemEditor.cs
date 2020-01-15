using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BreathingSystem))]
public class BreathingSystemEditor : Editor
{
    SerializedProperty canWalkDuringBreathing;
    SerializedProperty walkSpeedDuringBreathing;
    private void OnEnable()
    {
        canWalkDuringBreathing = serializedObject.FindProperty("canWalkDuringBreathing");
        walkSpeedDuringBreathing = serializedObject.FindProperty("walkSpeedDuringBreathing");

    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        BreathingSystem breathingSystem = (BreathingSystem)target;

        if (breathingSystem.canWalkDuringBreathing)
        {
            breathingSystem.walkSpeedDuringBreathing = EditorGUILayout.Slider("Walk speed during breathing ", walkSpeedDuringBreathing.floatValue, 0f,0.1f);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(breathingSystem);
        }
    }
}
