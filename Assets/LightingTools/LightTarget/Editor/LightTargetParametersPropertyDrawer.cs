using UnityEditor;
using UnityEngine;
using LightingTools;

[CustomPropertyDrawer(typeof(LightTargetParameters))]
public class LightTargetParametersPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        EditorGUILayout.PropertyField(property.FindPropertyRelative("name"));

        EditorGUILayout.LabelField("Rig", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.PropertyField(property.FindPropertyRelative("Yaw"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("Pitch"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("Roll"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("distance"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("offset"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("linkToCameraRotation"));
        EditorGUILayout.EndVertical();

        EditorGUILayout.LabelField("Light", EditorStyles.boldLabel);
		EditorGUILayout.BeginVertical("box");
        EditorGUILayout.PropertyField(property.FindPropertyRelative("intensity"), new GUIContent("Intensity / Color"), GUILayout.MaxWidth(EditorGUIUtility.labelWidth + 80));
        var propertyRect = GUILayoutUtility.GetLastRect();
        var colorRect = new Rect(propertyRect.x + EditorGUIUtility.labelWidth + 100, propertyRect.y, position.xMax * 0.3f, EditorGUIUtility.singleLineHeight);
        EditorGUILayout.PropertyField(property.FindPropertyRelative("useColorTemperature"));
        if (property.FindPropertyRelative("useColorTemperature").boolValue == true)
        {
            EditorGUI.PropertyField(colorRect, property.FindPropertyRelative("colorTemperature"), GUIContent.none);
        }
        if (property.FindPropertyRelative("useColorTemperature").boolValue == false)
        {
            EditorGUI.PropertyField(colorRect, property.FindPropertyRelative("colorFilter"), GUIContent.none);
        }
        EditorGUILayout.PropertyField(property.FindPropertyRelative("mode"), new GUIContent("Light mode / Indirect Intensity"), GUILayout.MaxWidth(EditorGUIUtility.labelWidth + 80));
        //EditorGUILayout.PropertyField(property.FindPropertyRelative("indirectIntensity"));
        var modeRect = GUILayoutUtility.GetLastRect();
        var indirectRect = new Rect(modeRect.x + EditorGUIUtility.labelWidth + 100, modeRect.y, position.xMax * 0.3f, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(indirectRect, property.FindPropertyRelative("indirectIntensity"), GUIContent.none);
        EditorGUILayout.PropertyField(property.FindPropertyRelative("range"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("lightCookie"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("lightAngle"));
		EditorGUILayout.EndVertical();
        
        // Draw fields
        EditorGUILayout.LabelField("Shadows", EditorStyles.boldLabel);
		EditorGUILayout.BeginVertical("box");
        EditorGUILayout.PropertyField(property.FindPropertyRelative("shadows"));
        if (property.FindPropertyRelative("shadows").enumValueIndex != 0)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("shadowQuality"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("ShadowNearClip"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("shadowBias"));
        }
		EditorGUILayout.EndVertical();

        EditorGUILayout.LabelField("Visualization", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(property.FindPropertyRelative("drawGizmo"));

        EditorGUI.EndProperty();
        
    }
}