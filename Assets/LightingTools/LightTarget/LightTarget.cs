using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using LightingTools;
#if UNITY_2017
using UnityEngine.Experimental.Rendering;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class LightTarget : MonoBehaviour
{
    public LightTargetParameters lightTargetParameters;
    [SerializeField][HideInInspector]
    private GameObject targetedLight;
    [SerializeField][HideInInspector]
    private GameObject LightParent;
    private bool showEntities = false;

    private void OnEnable()
    {
        if (LightParent == null || LightParent.transform.parent != gameObject.transform) { CreateLightParent(); }
        if (targetedLight == null || LightParent.transform.parent != gameObject.transform) { CreateLight(); }
        //Enable if it has been disabled
        if (targetedLight != null) { targetedLight.GetComponent<Light>().enabled = true; }
        Update();
    }

    private void OnDisable()
    {
        targetedLight.GetComponent<Light>().enabled = false;
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (LightParent != null) { SetLightParentTransform(); }
		if (targetedLight != null && lightTargetParameters!= null )
        {
            SetLightTransform();
            SetLightSettings();
        }
        ApplyShowFlags(showEntities);
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if(lightTargetParameters!= null && lightTargetParameters.drawGizmo)
        {
            var targetedLightSpot = targetedLight.GetComponent<Light>();
            var gizmoColor = lightTargetParameters.colorFilter;
            gizmoColor.a = 1f;
            Gizmos.color = gizmoColor;
            var relativeRotation = targetedLight.transform.rotation ;
            var relativePosition = targetedLight.transform.position;
            Gizmos.matrix = targetedLight.transform.localToWorldMatrix; ;
            Gizmos.DrawFrustum(new Vector3(0,0,1)*targetedLightSpot.shadowNearPlane, targetedLightSpot.spotAngle, targetedLightSpot.range, targetedLightSpot.shadowNearPlane, 1);
            Gizmos.matrix = Matrix4x4.identity;
            //Draw Target
            Gizmos.DrawLine(LightParent.transform.position, LightParent.transform.position + LightParent.transform.TransformVector(LightParent.transform.root.forward*0.25f));
            Gizmos.DrawLine(LightParent.transform.position, LightParent.transform.position + LightParent.transform.TransformVector(LightParent.transform.root.forward * -0.25f));
            Gizmos.DrawLine(LightParent.transform.position, LightParent.transform.position + LightParent.transform.TransformVector(LightParent.transform.root.up * 0.25f));
            Gizmos.DrawLine(LightParent.transform.position, LightParent.transform.position + LightParent.transform.TransformVector(LightParent.transform.root.up * -0.25f));
            Gizmos.DrawLine(LightParent.transform.position, LightParent.transform.position + LightParent.transform.TransformVector(LightParent.transform.root.right * 0.25f));
            Gizmos.DrawLine(LightParent.transform.position, LightParent.transform.position + LightParent.transform.TransformVector(LightParent.transform.root.right * -0.25f));
        }
    }
#endif

    void CreateLightParent()
    {
        LightParent = new GameObject("LightParent");
        LightParent.transform.parent = gameObject.transform;
        LightParent.transform.position = Vector3.zero;
        LightParent.transform.rotation = Quaternion.identity;
    }

    void CreateLight()
    {
        targetedLight = new GameObject("TargetSpot");
        targetedLight.transform.parent = LightParent.transform;
        var targetedLightSpot = targetedLight.AddComponent<Light>();
        targetedLight.AddComponent<AdditionalLightData>();
        targetedLightSpot.type = LightType.Spot;
    }

    void SetLightTransform()
    {
        targetedLight.transform.localPosition = Vector3.forward * lightTargetParameters.distance;
        targetedLight.transform.localRotation = Quaternion.Euler(0,180,0);
    }

    void SetLightParentTransform()
    {
		if (LightParent != null && lightTargetParameters!=null && LightParent.transform.parent == gameObject.transform)
        {
			if ( lightTargetParameters.linkToCameraRotation)
            {
                var cameraRotation = FindObjectOfType<Camera>().transform.rotation;
                LightParent.transform.rotation = Quaternion.Euler(new Vector3(lightTargetParameters.Pitch, lightTargetParameters.Yaw, lightTargetParameters.Roll)) * cameraRotation;
            }
			if ( !lightTargetParameters.linkToCameraRotation)
            {
                LightParent.transform.rotation = Quaternion.Euler(new Vector3(lightTargetParameters.Pitch, lightTargetParameters.Yaw, lightTargetParameters.Roll));
            }
            LightParent.transform.localPosition = lightTargetParameters.offset;
        }
    }

    void SetLightSettings()
    {
        var targetedLightSpot = targetedLight.GetComponent<Light>();
#if UNITY_EDITOR
        switch (lightTargetParameters.mode)
        {
            case LightmapPresetBakeType.Realtime: targetedLightSpot.lightmapBakeType = LightmapBakeType.Realtime; break;
            case LightmapPresetBakeType.Baked: targetedLightSpot.lightmapBakeType = LightmapBakeType.Baked; break;
            case LightmapPresetBakeType.Mixed: targetedLightSpot.lightmapBakeType = LightmapBakeType.Mixed; break;
        }
#endif

        targetedLightSpot.shadows = lightTargetParameters.shadows;
        targetedLightSpot.intensity = lightTargetParameters.intensity;
        targetedLightSpot.color = lightTargetParameters.colorFilter;
        targetedLightSpot.range = lightTargetParameters.range;
        targetedLightSpot.shadowBias = lightTargetParameters.shadowBias;
        targetedLightSpot.spotAngle = lightTargetParameters.lightAngle;
        targetedLightSpot.shadowNearPlane = lightTargetParameters.ShadowNearClip;
/*
        switch (lightTargetParameters.shadowQuality)
        {
            case LightingTools.ShadowQuality.VeryHigh: targetedLightSpot.shadowResolution = UnityEngine.Rendering.LightShadowResolution.VeryHigh; break;
            case LightingTools.ShadowQuality.High: targetedLightSpot.shadowResolution = UnityEngine.Rendering.LightShadowResolution.High; break;
            case LightingTools.ShadowQuality.Medium: targetedLightSpot.shadowResolution = UnityEngine.Rendering.LightShadowResolution.Medium; break;
            case LightingTools.ShadowQuality.Low: targetedLightSpot.shadowResolution = UnityEngine.Rendering.LightShadowResolution.Low; break;
        }*/

#if UNITY_2017
        switch (lightTargetParameters.shadowQuality)
        {
            case LightingTools.ShadowQuality.VeryHigh: targetedLightSpot.GetComponent<AdditionalLightData>().shadowResolution = 2048; break;
            case LightingTools.ShadowQuality.High: targetedLightSpot.GetComponent<AdditionalLightData>().shadowResolution = 1024; break;
            case LightingTools.ShadowQuality.Medium: targetedLightSpot.GetComponent<AdditionalLightData>().shadowResolution = 512; break;
            case LightingTools.ShadowQuality.Low: targetedLightSpot.GetComponent<AdditionalLightData>().shadowResolution = 256; break;
        }
#endif
        if (targetedLightSpot.cookie != null) { targetedLightSpot.cookie = lightTargetParameters.lightCookie;}
    }

    private void OnDestroy()
    {
        DestroyImmediate(targetedLight);
        DestroyImmediate(LightParent);
    }

    void ApplyShowFlags(bool show)
    {
        if (targetedLight != null)
        {
            if (!show) { targetedLight.hideFlags = HideFlags.HideInHierarchy; }
            if (show)
            {
                targetedLight.hideFlags = HideFlags.None;
            }
        }
        if (LightParent != null)
        {
            if (!show) { LightParent.hideFlags = HideFlags.HideInHierarchy; }
            if (show)
            {
                LightParent.hideFlags = HideFlags.None;
            }
        }
    }

}
