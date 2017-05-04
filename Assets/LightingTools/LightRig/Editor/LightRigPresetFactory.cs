using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor.ProjectWindowCallback;
using System.IO;

public class LightRigPresetFactory
{
    [MenuItem("Assets/Create/Light Rig Preset", priority = 301)]
    private static void MenuCreateLightRigPreset()
    {
        var icon = EditorGUIUtility.FindTexture("ScriptableObject Icon");
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateLightRigPreset>(), "NewLightSourcePreset.asset", icon, null);
    }

    internal static LightRigPreset CreateLightRigPreset(string path)
    {
        LightRigPreset preset = ScriptableObject.CreateInstance<LightRigPreset>();
        preset.name = Path.GetFileName(path);
        AssetDatabase.CreateAsset(preset, path);
        return preset;
    }
}

internal class DoCreateLightRigPreset : EndNameEditAction
{
    public override void Action(int instanceId, string pathName, string resourceFile)
    {
        LightRigPreset preset = LightRigPresetFactory.CreateLightRigPreset(pathName);
        ProjectWindowUtil.ShowCreatedAsset(preset);
    }
}