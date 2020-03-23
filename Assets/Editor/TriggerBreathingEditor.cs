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
    SerializedProperty doCameraShake;
    SerializedProperty shakeIntensity;
    SerializedProperty tree;

    private void OnEnable()
    {
        canWalkDuringBreathing = serializedObject.FindProperty("canWalkDuringBreathing");
        walkSpeedDuringBreathing = serializedObject.FindProperty("walkSpeedDuringBreathing");
        breathingUnits = serializedObject.FindProperty("breathingUnits");
        requiredTimeSpendOutsideBounds = serializedObject.FindProperty("requiredTimeSpendOutsideBounds");
        requiredTimeSpendInsideBounds = serializedObject.FindProperty("requiredTimeSpendInsideBounds");
        requiredFailedToLose = serializedObject.FindProperty("requiredFailedToLose");
        doCameraShake = serializedObject.FindProperty("doCameraShake");
        shakeIntensity = serializedObject.FindProperty("shakeIntensity");
        tree = serializedObject.FindProperty("tree");
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TriggerBreathing triggerBreathing = (TriggerBreathing)target;
        //Si le booléen est vrai

        //if (triggerBreathing.animType == AnimType.CHOPPING)
        //{
        //    EditorGUILayout.PropertyField(tree, new GUIContent("Arbre à couper : "));
        //}

        if (triggerBreathing.canWalkDuringBreathing)
        {
            //Crée un slider dans l'editeur
            triggerBreathing.walkSpeedDuringBreathing =
                EditorGUILayout.Slider("Walk speed during breathing ", walkSpeedDuringBreathing.floatValue, 0f, 180f);
        }
        if (triggerBreathing.doCameraShake)
        {
            triggerBreathing.shakeIntensity = EditorGUILayout.Slider("Camera shake intensity", shakeIntensity.floatValue, 0f, 15f);
        }

        if (triggerBreathing.breathingUnits.Length == 1)
        {
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
