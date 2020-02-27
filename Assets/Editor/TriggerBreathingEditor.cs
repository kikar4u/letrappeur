using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TriggerBreathing))]
public class TriggerBreathingEditor : Editor
{
    SerializedProperty canWalkDuringBreathing;
    SerializedProperty walkSpeedDuringBreathing;
    SerializedProperty breathingUnits;
    SerializedProperty requiredTimeSpendOutsideBounds;
    SerializedProperty requiredTimeSpendInsideBounds;
    SerializedProperty requiredFailedToLose;

    private void OnEnable()
    {
        canWalkDuringBreathing = serializedObject.FindProperty("canWalkDuringBreathing");
        walkSpeedDuringBreathing = serializedObject.FindProperty("walkSpeedDuringBreathing");
        breathingUnits = serializedObject.FindProperty("breathingUnits");
        requiredTimeSpendOutsideBounds = serializedObject.FindProperty("requiredTimeSpendOutsideBounds");
        requiredTimeSpendInsideBounds = serializedObject.FindProperty("requiredTimeSpendInsideBounds");
        requiredFailedToLose = serializedObject.FindProperty("requiredFailedToLose");
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TriggerBreathing triggerBreathing = (TriggerBreathing)target;

        if (triggerBreathing.canWalkDuringBreathing)
        {
            triggerBreathing.walkSpeedDuringBreathing = EditorGUILayout.Slider("Walk speed during breathing ", walkSpeedDuringBreathing.floatValue, 0f, 180f);
        }

        if (triggerBreathing.breathingUnits.Length == 1)
        {
            //triggerBreathing.requiredTimeSpendOutsideBounds = EditorGUILayout.FloatField("Required time outside bounds to lose   :", requiredTimeSpendOutsideBounds.floatValue);
            //triggerBreathing.requiredTimeSpendInsideBounds = EditorGUILayout.FloatField("Required time inside bounds to win   :", requiredTimeSpendInsideBounds.floatValue);

            EditorGUILayout.PropertyField(requiredTimeSpendOutsideBounds, new GUIContent("Temps nécessaire pour perdre"));
            EditorGUILayout.PropertyField(requiredTimeSpendInsideBounds, new GUIContent("Temps nécessaire pour réussir"));
        }
        else if (triggerBreathing.breathingUnits.Length > 1)
        {
            EditorGUILayout.PropertyField(requiredFailedToLose, new GUIContent("Nbr de pattern raté pour perdre"));
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(triggerBreathing);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
