using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LightsVisualizer : MonoBehaviour
{
	private Light[] sceneLights;
	private ReflectionProbe[] sceneReflectionProbes;
	
	public bool showPointLights;
	public bool showSpotLights;
	public bool showReflectionProbes;

#if UNITY_EDITOR
    // This function is called when the object becomes enabled and active.
    protected void OnEnable()
	{
		fillArrays();
	}
	
	void fillArrays()
	{
		sceneLights = FindObjectsOfType<Light>();
		sceneReflectionProbes = FindObjectsOfType<ReflectionProbe>();
	}
	
	
    // Use this for initialization
    private void OnDrawGizmos()
    {
	    fillArrays();
	    if (showPointLights)
	    {
	    	DrawPointLightRange();
	    }
	    if (showSpotLights)
	    {
	    	DrawSpotLightRange();
	    }
	    if (showReflectionProbes)
	    {
	    	DrawReflectionProbesVolumes();
	    }
	    
    }
 
	public void DrawPointLightRange()
	{
		if (sceneLights.Length > 0)
		{
			foreach(Light light in sceneLights)
			{
				if (light.gameObject != Selection.activeGameObject)
				{
					if (light.type == LightType.Point)
					{
						Gizmos.color = new Color(1,1,1,0.2f);
						Gizmos.matrix = Matrix4x4.identity;
						Gizmos.DrawWireSphere(light.transform.position, light.range);
                        if (light.lightmapBakeType != LightmapBakeType.Baked)
						{
							if(light.shadows == LightShadows.None)
							{
								Gizmos.color = new Color(0,1,0,0.15f);
							}
							if(light.shadows != LightShadows.None)
							{
								Gizmos.color = new Color(1,0,0,0.15f);
								if ( light.lightmapBakeType == LightmapBakeType.Mixed)
								{
									Gizmos.color = new Color(1,1,0,0.15f);
								}
							}
							Gizmos.DrawSphere(light.transform.position,light.range);
						}
					}
				}
			}
		}
	}
		
	public void DrawSpotLightRange()
	{
		if (sceneLights.Length > 0)
		{
			foreach(Light light in sceneLights)
			{
				if (light.gameObject != Selection.activeGameObject)
				{
					if (light.type == LightType.Spot)
					{
						Gizmos.color = new Color(1,1,1,0.2f);
						Gizmos.matrix = light.transform.localToWorldMatrix;
						Gizmos.DrawFrustum(new Vector3(0,0,1)*light.shadowNearPlane,light.spotAngle,light.range,light.shadowNearPlane,1); 
						if (light.lightmapBakeType !=	LightmapBakeType.Baked)
						{
							if(light.shadows == LightShadows.None)
							{
								Gizmos.color = new Color(0,1,0,0.15f);
							}
							if(light.shadows != LightShadows.None)
							{
								Gizmos.color = new Color(1,0,0,0.15f);
								if ( light.lightmapBakeType == LightmapBakeType.Mixed)
								{
									Gizmos.color = new Color(1,1,0,0.15f);
								}
							}
							for (int i=1; i<=5;i++)
							{
								var sectionDistance = Mathf.Lerp(light.range,light.shadowNearPlane,((float)i-1f)*0.25f);
								var cubesize = sectionDistance * Mathf.Tan(light.spotAngle*Mathf.Deg2Rad);
								Gizmos.DrawCube(Vector3.forward*sectionDistance,new Vector3(cubesize,cubesize,0.1f));	
							}					
						}
					}
				}
			}
		}
	}
	
	public void DrawReflectionProbesVolumes()
	{
		if (sceneReflectionProbes.Length > 0)
		{
			foreach ( ReflectionProbe probe in sceneReflectionProbes)
			{
				if (probe.gameObject != Selection.activeGameObject)
				{
					Gizmos.color = new Color(1,0.93f,0.40f,0.4f);
					Gizmos.matrix = Matrix4x4.identity;
					Gizmos.DrawWireCube(probe.transform.position + probe.center,probe.size);
					if(probe.mode ==	UnityEngine.Rendering.ReflectionProbeMode.Realtime)
					{
						Gizmos.color = new Color(1,0.93f,0.4f,0.15f);
						if (probe.refreshMode ==	UnityEngine.Rendering.ReflectionProbeRefreshMode.EveryFrame)
						{
							Gizmos.color = new Color(1,0,0,0.15f);
						}
						Gizmos.DrawCube(probe.transform.position + probe.center,probe.size);
					}
				}
			}
		}
	}

#endif
}
