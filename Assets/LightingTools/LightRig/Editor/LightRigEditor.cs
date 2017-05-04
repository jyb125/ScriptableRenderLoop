using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LightingTools;
using System;
using Object = UnityEngine.Object;

[CustomEditor(typeof(LightRig))]
public class LightRigEditor : Editor
{
    [SerializeField]
    private LightRig targetLightRig;
    public int lightCount = 0;

    SerializedProperty lightTargetParametersList;
    SerializedProperty focusedLightTarget;
    SerializedProperty ifocus;

    

    void OnEnable()
    {
        targetLightRig = (LightRig)serializedObject.targetObject;
        targetLightRig.FillLists();
        lightTargetParametersList = serializedObject.FindProperty("lightTargetParametersList");
        ifocus = serializedObject.FindProperty("focus");
        lightCount = lightTargetParametersList.arraySize;
        if (ifocus.intValue != -1) changeFocus();
    }

    public override void OnInspectorGUI()
    {
        
        serializedObject.Update();
        //DrawDefaultInspector ();

        EditorGUILayout.Space();
        GUILayout.BeginVertical("TextArea");
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        for (int i=0; i<lightCount;i++)
        {
            var currentLightTarget = lightTargetParametersList.GetArrayElementAtIndex(i);
            var lightTargetName = currentLightTarget.FindPropertyRelative("name").stringValue;
            if (GUILayout.Button(new GUIContent(lightTargetName), EditorStyles.toolbarButton))
            {
                ifocus.intValue = i;
                serializedObject.ApplyModifiedProperties();
                changeFocus();
            }
        }
        if (GUILayout.Button(new GUIContent("+"), EditorStyles.toolbarButton))
        {
            targetLightRig.AddTargetedLight();
            serializedObject.Update();
            lightCount = lightTargetParametersList.arraySize;
            ifocus.intValue = lightCount - 1;
            serializedObject.ApplyModifiedProperties();
            changeFocus();
        }
        if (GUILayout.Button(new GUIContent("-"), EditorStyles.toolbarButton))
        {
            targetLightRig.RemoveTargetedLight();
            serializedObject.Update();
            lightCount = lightTargetParametersList.arraySize;
            ifocus.intValue = lightCount - 1;
            serializedObject.ApplyModifiedProperties();
            changeFocus();
        }
        if (GUILayout.Button(new GUIContent("Load preset"), EditorStyles.toolbarButton))
        {
            GenericMenu toolsMenu = new GenericMenu();
            var presets = GatherLightRigPresets();
            foreach (string preset in presets)
            {
                var path = AssetDatabase.GUIDToAssetPath(preset);
                var filename = System.IO.Path.GetFileNameWithoutExtension(path);
                toolsMenu.AddItem(new GUIContent(filename), false, applyPreset, AssetDatabase.GUIDToAssetPath(preset));
            }
            //show at mouse pos
            toolsMenu.ShowAsContext();
        }
        
        if (GUILayout.Button(new GUIContent("Save preset"), EditorStyles.toolbarButton))
        {
            targetLightRig.FillLists();
            lightCount = lightTargetParametersList.arraySize;
            serializedObject.ApplyModifiedProperties();
            var newPreset = targetLightRig.SaveLightRigProperties();
            var path = EditorUtility.SaveFilePanelInProject("Save light parameters as preset", "lightrig", "asset", "");
            if (path == "") return;
            Debug.Log("Created profile" + path);
            AssetDatabase.CreateAsset(newPreset, path);
            AssetDatabase.SaveAssets();
        }
        
        GUILayout.EndHorizontal();

        if (ifocus.intValue != -1 && focusedLightTarget != null) EditorGUILayout.PropertyField(focusedLightTarget, true);
        GUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
    }

    void changeFocus()
    {
        if (ifocus.intValue == -1) return;
        Debug.Log("focus="+ ifocus.intValue);
        lightTargetParametersList = serializedObject.FindProperty("lightTargetParametersList");
        serializedObject.Update();
        focusedLightTarget = lightTargetParametersList.GetArrayElementAtIndex(ifocus.intValue);
    }

    private string[] GatherLightRigPresets()
    {
        var presetsList = AssetDatabase.FindAssets("t:LightRigPreset");
        return presetsList;
    }

    void applyPreset(object presetPath)
    {
        
        var preset = (LightRigPreset)AssetDatabase.LoadAssetAtPath((string)presetPath, typeof(LightRigPreset));
        var presetLightCount = preset.lightTargetParametersList.Count;
        if (presetLightCount < 1)
        {
            Debug.LogWarning("Empty preset");
            return;
        }
        Debug.Log("Loading preset at "+ presetPath);
        for (int i = 0; i< lightTargetParametersList.arraySize; i++)
        {
            targetLightRig.RemoveTargetedLight();
        }
        for (int i = 0; i < presetLightCount; i++)
        {
            targetLightRig.AddTargetedLight();
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
            var fields = typeof(LightTargetParameters).GetFields();
            foreach (var field in fields)
            {
                var subProperty = lightTargetParametersList.GetArrayElementAtIndex(i).FindPropertyRelative(field.Name);
                if (field.GetValue(preset.lightTargetParametersList[i]) != null)
                {
                    var value = field.GetValue(preset.lightTargetParametersList[i]);
                    LightingUtilities.AssignSerializedProperty(subProperty, value);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
        targetLightRig.FillLists();
        lightCount = lightTargetParametersList.arraySize;
        ifocus.intValue = 0;
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
        changeFocus();
    }
}
