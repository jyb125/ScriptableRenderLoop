using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[CustomEditor(typeof(LightRigPreset))]
public class LightRigPresetEditor : Editor
{
    //Data from BakerAssetData
    public SerializedProperty lightSourceParameters;

    public void OnEnable()
    {
        lightSourceParameters = serializedObject.FindProperty("lightTargetParametersList");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        serializedObject.ApplyModifiedProperties();
    }
}