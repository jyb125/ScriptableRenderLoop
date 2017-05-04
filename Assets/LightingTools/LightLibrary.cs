using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using Object = UnityEngine.Object;

namespace LightingTools
{
    [System.Serializable]
    public enum LightmapPresetBakeType
    {
        //Simplify serialization
        Baked = 0,
        Mixed = 1,
        Realtime = 2
    }

    public enum ShadowQuality
    {
        FromQualitySettings = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        VeryHigh = 4
    }

    [System.Serializable]
    public enum LightShape
    {
        Point,
        Spot,
        Directional,
        Rectangle,
        Sphere,
        Line,
        Disc,
        Frustum
    }

    [System.Serializable]
	public class LightParameters
	{
        public string name = "Light" ;
        public LightType type = LightType.Point;
        public LightmapPresetBakeType mode = LightmapPresetBakeType.Mixed;
		public float range = 8;
		public bool useColorTemperature;
		public float colorTemperature = 6500;
		public Color colorFilter = Color.white;
		public float intensity = 1;
		public float indirectIntensity = 1;
		public float lightAngle = 45;
		public LightShadows shadows = LightShadows.Soft ;
		[Range(0.01f,10f)]
		public float ShadowNearClip = 0.1f;
		public Texture2D lightCookie;
		public float cookieSize = 5 ;
        //HDRenderPipelineParameters
#if UNITY_2017
        public LightShape shape = LightShape.Point;
        public float innerConeAngle = 40;
        public float length;
        public float width;
        public float fadeDistance = 50;
        public float shadowFadeDistance = 10 ;
        public bool affectDiffuse = true;
        public bool affectSpecular = true;
#endif
    }

    [System.Serializable]
    public class LightTargetParameters
    {
        public string name = "Light01";
        [Range(-180f, 180f)]
        public float Yaw = 0.1f;
        [Range( 90f, -90f)]
        public float Pitch = 0.1f;
        [Range(-180f, 180f)]
        public float Roll = 0.1f;
        public bool linkToCameraRotation = false;
        public float distance = 2f;
        public Vector3 offset;
        public LightmapPresetBakeType mode = LightmapPresetBakeType.Realtime;
        public float range = 3;
        public bool useColorTemperature;
        public float colorTemperature = 6500;
        public Color colorFilter = Color.white;
        public float intensity = 1;
        public float indirectIntensity = 1;
        [Range(0f, 180f)]
        public float lightAngle = 45;
        public LightShadows shadows = LightShadows.Soft;
        //public LightShadowResolution shadowsResolution = LightShadowResolution.High;
        public ShadowQuality shadowQuality = ShadowQuality.High;
        [Range(0.01f, 10f)]
        public float ShadowNearClip = 0.5f;
        [Range(0f, 2f)]
        public float shadowBias = 0.005f;
        public Texture2D lightCookie;
        public bool drawGizmo = false;
    }

    [System.Serializable]
	public class LightSourceMeshParameters
	{
		public GameObject lightSourceObject;
		public ShadowCastingMode meshShadowMode;
		public bool showObjectInHierarchy;
	}
	
	[System.Serializable]
	public class LightSourceMaterialsParameters
	{
		public bool linkEmissiveIntensityWithLight = true ;
		public bool linkEmissiveColorWithLight = true ;
		public float emissiveMultiplier = 1f;
	}

    [System.Serializable]
    public class LightSourceAnimationParameters
    {
	    public bool enabledFromStart = true ;
	    public bool enableFunctionalAnimation = false ;
	    public bool enableSwithOnAnimation = false ;
	    public bool enableSwithOffAnimation = false ;
	    public bool enableBreakAnimation = false ;

        public LightAnimationParameters functionalAnimationParameters;
	    public LightAnimationParameters switchOnAnimationParameters;
	    public LightAnimationParameters switchOffAnimationParameters;
	    public LightAnimationParameters breakAnimationParameters;
    }

    [System.Serializable]
    public class LightAnimationParameters
    {
        public AnimationMode animationMode;
        public AnimationCurve animationCurve;
        public AnimationClip animationClip;
        public NoiseAnimationParameters noiseParameters;
	    public float animationLength = 1;
    }

    [System.Serializable]
    public class NoiseAnimationParameters
    {
        public float frequency = 5;
        public float minimumValue = 0;
        public float maximumValue = 1;
        [Range(0.0f, 1.0f)]
        public float jumpFrequency = 0;
    }

    public enum AnimationMode {Curve, Noise, AnimationClip }

#if UNITY_EDITOR
    public static class LightingUtilities
    {
        public static void AssignSerializedProperty(SerializedProperty sp, object source)
        {
            var valueType = source.GetType();
            if (valueType.IsEnum)
            {
                sp.enumValueIndex = (int)source;
            }
            else if (valueType == typeof(Color))
            {
                sp.colorValue = (Color)source;
            }
            else if (valueType == typeof(float))
            {
                sp.floatValue = (float)source;
            }
            else if (valueType == typeof(Vector3))
            {
                sp.vector3Value = (Vector3)source;
            }
            else if (valueType == typeof(bool))
            {
                sp.boolValue = (bool)source;
            }
            else if (valueType == typeof(string))
            {
                sp.stringValue = (string)source;
            }
            else if (typeof(int).IsAssignableFrom(valueType))
            {
                sp.intValue = (int)source;
            }
            else if (typeof(Object).IsAssignableFrom(valueType))
            {
                sp.objectReferenceValue = (Object)source;
            }
            else
            {
                Debug.LogError("Missing type : " + valueType);
            }
        }
    }
#endif
}