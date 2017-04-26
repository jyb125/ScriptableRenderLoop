﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnityEngine.Experimental.Rendering.HDPipeline
{
    [GenerateHLSL]
    public enum DebugDisplayMode
    {
        None,
        ViewMaterial,
        DiffuseLighting,
        SpecularLighting,
        VisualizeCascade
    }

    [Serializable]
    public class DebugDisplaySettings
    {
        public float debugOverlayRatio = 0.33f;
        public bool displayMaterialDebug = false;
        public bool displayRenderingDebug = false;
        public bool displayLightingDebug = false;

        public DebugDisplayMode debugDisplayMode = DebugDisplayMode.None;

        public MaterialDebugSettings materialDebugSettings = new MaterialDebugSettings();
        public LightingDebugSettings lightingDebugSettings = new LightingDebugSettings();
        public RenderingDebugSettings renderingDebugSettings = new RenderingDebugSettings();

        public bool IsDebugDisplayEnable()
        {
            return debugDisplayMode != DebugDisplayMode.None;
        }
        

        public void RegisterDebug()
        {
            DebugMenuManager.instance.AddDebugItem<float>("Display Stats", "Frame Rate", () => 1.0f / Time.deltaTime, null, true);
            DebugMenuManager.instance.AddDebugItem<float>("Display Stats", "Frame Time", () => Time.deltaTime * 1000.0f, null, true);

            DebugMenuManager.instance.AddDebugItem<LightingDebugMenu, bool>("Enable Shadows", () => lightingDebugSettings.enableShadows, (value) => lightingDebugSettings.enableShadows = (bool)value);
            DebugMenuManager.instance.AddDebugItem<LightingDebugMenu, ShadowMapDebugMode>("Shadow Debug Mode", () => lightingDebugSettings.shadowDebugMode, (value) => lightingDebugSettings.shadowDebugMode = (ShadowMapDebugMode)value);
            DebugMenuManager.instance.AddDebugItem<LightingDebugMenu, uint>("Shadow Map Index", () => lightingDebugSettings.shadowMapIndex, (value) => lightingDebugSettings.shadowMapIndex = (uint)value);
            DebugMenuManager.instance.AddDebugItem<LightingDebugMenu, LightingDebugMode>("Lighting Debug Mode", () => lightingDebugSettings.lightingDebugMode, (value) => lightingDebugSettings.lightingDebugMode = (LightingDebugMode)value);
            DebugMenuManager.instance.AddDebugItem<LightingDebugMenu, bool>("Override Smoothness", () => lightingDebugSettings.overrideSmoothness, (value) => lightingDebugSettings.overrideSmoothness = (bool)value);
            DebugMenuManager.instance.AddDebugItem<LightingDebugMenu, float>("Override Smoothness Value", () => lightingDebugSettings.overrideSmoothnessValue, (value) => lightingDebugSettings.overrideSmoothnessValue = (float)value, false, new DebugItemDrawFloatMinMax(0.0f, 1.0f));
            DebugMenuManager.instance.AddDebugItem<LightingDebugMenu, Color>("Debug Lighting Albedo", () => lightingDebugSettings.debugLightingAlbedo, (value) => lightingDebugSettings.debugLightingAlbedo = (Color)value);
            DebugMenuManager.instance.AddDebugItem<bool>("Lighting", "Display Sky Reflection", () => lightingDebugSettings.displaySkyReflection, (value) => lightingDebugSettings.displaySkyReflection = (bool)value);
            DebugMenuManager.instance.AddDebugItem<LightingDebugMenu, float>("Sky Reflection Mipmap", () => lightingDebugSettings.skyReflectionMipmap, (value) => lightingDebugSettings.skyReflectionMipmap = (float)value, false, new DebugItemDrawFloatMinMax(0.0f, 1.0f));

            DebugMenuManager.instance.AddDebugItem<bool>("Rendering", "Display Opaque",() => renderingDebugSettings.displayOpaqueObjects, (value) => renderingDebugSettings.displayOpaqueObjects = (bool)value);
            DebugMenuManager.instance.AddDebugItem<bool>("Rendering", "Display Transparency",() => renderingDebugSettings.displayTransparentObjects, (value) => renderingDebugSettings.displayTransparentObjects = (bool)value);
            DebugMenuManager.instance.AddDebugItem<bool>("Rendering", "Enable Distortion",() => renderingDebugSettings.enableDistortion, (value) => renderingDebugSettings.enableDistortion = (bool)value);
            DebugMenuManager.instance.AddDebugItem<bool>("Rendering", "Enable Subsurface Scattering",() => renderingDebugSettings.enableSSS, (value) => renderingDebugSettings.enableSSS = (bool)value);
        }

        public void OnValidate()
        {
            lightingDebugSettings.OnValidate();
        }
    }

    namespace Attributes
    {
        // 0 is reserved!
        [GenerateHLSL]
        public enum DebugViewVarying
        {
            Texcoord0 = 1,
            Texcoord1,
            Texcoord2,
            Texcoord3,
            VertexTangentWS,
            VertexBitangentWS,
            VertexNormalWS,
            VertexColor,
            VertexColorAlpha,
            // caution if you add something here, it must start below
        };

        // Number must be contiguous
        [GenerateHLSL]
        public enum DebugViewGbuffer
        {
            Depth = DebugViewVarying.VertexColorAlpha + 1,
            BakeDiffuseLightingWithAlbedoPlusEmissive,
        }
    }

    [Serializable]
    public class MaterialDebugSettings
    {
        public int debugViewMaterial = 0;
    }

    [Serializable]
    public class RenderingDebugSettings
    {
        public bool displayOpaqueObjects = true;
        public bool displayTransparentObjects = true;
        public bool enableDistortion = true;
        public bool enableSSS = true;
    }

    public enum ShadowMapDebugMode
    {
        None,
        VisualizeAtlas,
        VisualizeShadowMap
    }

    [Serializable]
    public class LightingDebugSettings
    {
        public bool                 enableShadows = true;
        public ShadowMapDebugMode   shadowDebugMode = ShadowMapDebugMode.None;
        public uint                 shadowMapIndex = 0;

        public bool                 overrideSmoothness = false;
        public float                overrideSmoothnessValue = 0.5f;
        public Color                debugLightingAlbedo = new Color(0.5f, 0.5f, 0.5f);

        public bool                 displaySkyReflection = false;
        public float                skyReflectionMipmap = 0.0f;

        public void OnValidate()
        {
            overrideSmoothnessValue = Mathf.Clamp(overrideSmoothnessValue, 0.0f, 1.0f);
            skyReflectionMipmap = Mathf.Clamp(skyReflectionMipmap, 0.0f, 1.0f);
        }
    }
}