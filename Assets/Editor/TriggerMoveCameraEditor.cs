using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TriggerMoveCamera))]
public class TriggerMoveCameraEditor : Editor
{
    SerializedProperty shake;
    SerializedProperty shakeI;
    SerializedProperty shakeF;

    private void OnEnable()
    {
        shake = serializedObject.FindProperty("shake");
        shakeI = serializedObject.FindProperty("shakeintensity");
        shakeF = serializedObject.FindProperty("shakeFrequency");
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TriggerMoveCamera triggerMoveCameraEditor = (TriggerMoveCamera)target;

        if (triggerMoveCameraEditor.shake)
        {
            triggerMoveCameraEditor.shakeFrequency =
     EditorGUILayout.Slider("Shake frequency", shakeF.floatValue, 0f, 10f);
            triggerMoveCameraEditor.shakeintensity =
     EditorGUILayout.Slider("Shake intensity", shakeI.floatValue, 0f, 10f);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(triggerMoveCameraEditor);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
