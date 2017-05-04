using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;

[CustomEditor(typeof(LightTarget))]
public class LightTargetInspector : Editor
{
    public LightTarget lightTarget;
    SerializedProperty lightTargetParameters;
    public List<Vector3> lightPivots;

    void OnEnable()
    {
        lightTarget = (LightTarget)serializedObject.targetObject;
        lightTargetParameters = serializedObject.FindProperty("lightTargetParameters");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(lightTargetParameters, true);
        serializedObject.ApplyModifiedProperties();
    }

    /*
    public void EditPivots(SceneView pivots)
    {
        if (Event.current.type == EventType.keyDown)
        {
            EditPivotHandleKeyboardShortcuts();
        }

        if (bakeInfo != null && previewGameObject != null && bakeInfo.pivots != null)
        {
            bakeInfoEditor.serializedObject.Update();
            int i = 0;
            foreach (PivotBakeInfo.Pivot p in bakeInfo.pivots)
            {
                var worldpos = previewGameObject.transform.TransformPoint(p.Position);

                Handles.color = Color.black;
                Handles.SphereHandleCap(0, worldpos, previewGameObject.transform.rotation, 0.333f, EventType.repaint);

                switch (p.Depth)
                {
                    case PivotBakeInfo.PivotDepth.Trunk: Handles.color = Color.red; break;
                    case PivotBakeInfo.PivotDepth.Branch: Handles.color = Color.yellow; break;
                    case PivotBakeInfo.PivotDepth.Leaf: Handles.color = Color.green; break;
                }

                if (i == SelectedPivot)
                {
                    Handles.SphereHandleCap(0, worldpos, previewGameObject.transform.rotation, 0.2f, EventType.repaint);
                }
                else
                {
                    if (Handles.Button(worldpos, previewGameObject.transform.rotation, 0.2f, 0.2f, Handles.SphereHandleCap))
                        SelectedPivot = i;
                }


                if (i == SelectedPivot)
                {
                    worldpos = Handles.PositionHandle(worldpos, previewGameObject.transform.rotation);
                    var outlocalpos = previewGameObject.transform.InverseTransformPoint(worldpos);
                    if (p.Position != outlocalpos)
                    {
                        Undo.RecordObject(bakeInfo, "Move Pivot");
                        p.Position = outlocalpos;
                        Repaint();
                    }

                    var radius = Handles.RadiusHandle(previewGameObject.transform.rotation, worldpos, p.SearchRadius);

                    if (ez != null && Event.current.type == EventType.repaint)
                        DrawPointsAround(outlocalpos, radius);

                    if (p.SearchRadius != radius)
                    {
                        Undo.RecordObject(bakeInfo, "Set Pivot Radius");
                        p.SearchRadius = radius;
                        Repaint();
                    }
                }

                Handles.Label(worldpos, "#" + i + " : " + p.Depth);
                i++;
            }

            // If nothing happened while clicking, unselect
            if (Event.current.type == EventType.MouseUp && Event.current.button > 0)
                SelectedPivot = -1;

            Handles.BeginGUI();
            GUI.Window(0, sceneWindowRect, EditPivotSceneWindow, "Edit Pivots");
            Handles.EndGUI();
        }
        else
        {
            Handles.BeginGUI();
            GUI.color = Color.red * 5;
            GUILayout.Label("   CONFIGURE BAKEINFO AND PREVIEW GAME OBJECT");
            Handles.EndGUI();
        }
    }
    */
}
