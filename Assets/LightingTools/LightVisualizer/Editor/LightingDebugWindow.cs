using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

public class LightingDebugWindow : EditorWindow
{
	public bool showPointLights;
	public bool showSpotLights;
	public bool showReflectionProbes;
	
	[MenuItem("Lighting/Lighting Debug Window")]

    static void Init()
    {
        // Get existing open window or if none, make a new one:
	    LightingDebugWindow window = (LightingDebugWindow)EditorWindow.GetWindow(typeof(LightingDebugWindow), true, "Lighting debug window");
        var sceneViewWindow = EditorWindow.GetWindow < SceneView >();
	    window.position = new Rect(sceneViewWindow.position.x+25, sceneViewWindow.position.y + 75, 400, 200 );
        window.Show();
	    Debug.Log("Started Window", window);
	    var visualizerGO = new GameObject();
	    visualizerGO.AddComponent<LightsVisualizer>();
	    visualizerGO.name = "lightVisualizer";
    }

    void OnGUI()
	{
		
		EditorGUILayout.HelpBox("Visualize lights in your scene. \nPointlights and spotlights : \nwire gizmo only = baked light, \ngreen filled gizmo = dynamic but no realtime shadow, \nred filled gizmo = dynamic and realtime shadows. \nReflection probes : \nwire gizmo = baked reflection probe, \nyellow filled gizmo = runtime reflection probe refreshed once, \nred filled gizmo = runtime reflection probe refreshed every frame", MessageType.Info);
		EditorGUI.BeginChangeCheck();
		showPointLights = EditorGUILayout.Toggle("Show point lights", showPointLights);
		showSpotLights = EditorGUILayout.Toggle("Show spot lights", showSpotLights);
		showReflectionProbes = EditorGUILayout.Toggle("Show reflection probes", showReflectionProbes);
		
		if (EditorGUI.EndChangeCheck ()) 
		{
			var visualizerComponent = FindObjectOfType<LightsVisualizer>();
			visualizerComponent.showPointLights = showPointLights;
			visualizerComponent.showSpotLights = showSpotLights;
			visualizerComponent.showReflectionProbes = showReflectionProbes;
			SceneView.FocusWindowIfItsOpen<SceneView>();
		}
	}
	
	// OnDestroy is called when the EditorWindow is closed.
	protected void OnDestroy()
	{
		DestroyImmediate(FindObjectOfType<LightsVisualizer>().gameObject);
	}
}