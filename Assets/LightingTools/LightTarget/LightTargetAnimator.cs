using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(LightTarget))]
public class LightTargetAnimator : MonoBehaviour {

    public bool animateOrientation;
    [Range(-180f, 180f)]
    public float Yaw = 0.1f;
    [Range(90f, -90f)]
    public float Pitch = 0.1f;
    [Range(-180f, 180f)]
    public float Roll = 0.1f;

	public bool animateTransform;
    public float distance = 2f;
    public Vector3 offset;
    
	public bool animateLightProperties;
	public float range = 3;
    public float colorTemperature = 6500;
    public Color colorFilter = Color.white;
    public float intensity = 1;
    public float indirectIntensity = 1;
    [Range(0f, 180f)]
    public float lightAngle = 45;
    public LightShadows shadows = LightShadows.Soft;
    public LightShadowResolution shadowsResolution = LightShadowResolution.High;
    public LightingTools.ShadowQuality shadowQuality = LightingTools.ShadowQuality.High;
    [Range(0.01f, 10f)]
    public float ShadowNearClip = 0.1f;
    [Range(0f, 2f)]
    public float shadowBias = 0.005f;

	private LightTarget lightTarget;

	void OnEnable()
	{
		lightTarget = GetComponent<LightTarget>();
		intensity = lightTarget.lightTargetParameters.intensity;
		indirectIntensity = lightTarget.lightTargetParameters.indirectIntensity;
		range = lightTarget.lightTargetParameters.range;
		colorTemperature =lightTarget.lightTargetParameters.colorTemperature;
		colorFilter=lightTarget.lightTargetParameters.colorFilter;
		lightAngle=lightTarget.lightTargetParameters.lightAngle;
		shadows=lightTarget.lightTargetParameters.shadows;
        //shadowsResolution=lightTarget.lightTargetParameters.shadowsResolution;
        shadowQuality = lightTarget.lightTargetParameters.shadowQuality;
        ShadowNearClip =lightTarget.lightTargetParameters.ShadowNearClip;
		shadowBias=lightTarget.lightTargetParameters.shadowBias;
	}

    // Use this for initialization
    void Start ()
    {
        lightTarget = GetComponent<LightTarget>();

    }
	
	// Update is called once per frame
	void Update ()
    {
		if (animateOrientation) {
			lightTarget.lightTargetParameters.Yaw = Yaw;
			lightTarget.lightTargetParameters.Pitch = Pitch;
			lightTarget.lightTargetParameters.Roll = Roll;
		}
		if (animateTransform) {
			lightTarget.lightTargetParameters.distance = distance;
			lightTarget.lightTargetParameters.offset = offset;
		}
		if (animateLightProperties) {
			lightTarget.lightTargetParameters.intensity = intensity;
			lightTarget.lightTargetParameters.indirectIntensity = indirectIntensity;
			lightTarget.lightTargetParameters.range = range;
			lightTarget.lightTargetParameters.colorTemperature = colorTemperature;
			lightTarget.lightTargetParameters.colorFilter = colorFilter;
			lightTarget.lightTargetParameters.lightAngle = lightAngle;
			lightTarget.lightTargetParameters.shadows = shadows;
			//lightTarget.lightTargetParameters.shadowsResolution = shadowsResolution;
            lightTarget.lightTargetParameters.shadowQuality = shadowQuality;
            lightTarget.lightTargetParameters.ShadowNearClip = ShadowNearClip;
			lightTarget.lightTargetParameters.shadowBias = shadowBias;
		}

	}
}
