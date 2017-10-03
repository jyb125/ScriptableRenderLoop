//-----------------------------------------------------------------------------
// SurfaceData and BSDFData
//-----------------------------------------------------------------------------

// SurfaceData is define in Eye.cs which generate Eye.cs.hlsl
#include "Eye.cs.hlsl"
#include "../Lit/SubsurfaceScatteringProfile.cs.hlsl"

#define WANT_SSS_CODE
#include "../LightEvaluationShare1.hlsl"

void FillMaterialIdStandardData(float3 baseColor, float specular, float metallic, float roughness, float3 normalWS, float3 tangentWS, float anisotropy, inout BSDFData bsdfData)
{
	bsdfData.diffuseColor = baseColor*0.5;
	bsdfData.fresnel0 =1;

	// TODO: encode specular

	bsdfData.tangentWS = tangentWS;
	bsdfData.bitangentWS = cross(normalWS, tangentWS);
	ConvertAnisotropyToRoughness(roughness, anisotropy, bsdfData.roughnessT, bsdfData.roughnessB);
	bsdfData.anisotropy = 0;
}

//-----------------------------------------------------------------------------
// conversion function for forward
//-----------------------------------------------------------------------------

BSDFData ConvertSurfaceDataToBSDFData(SurfaceData surfaceData)
{
    ApplyDebugToSurfaceData(surfaceData);

	BSDFData bsdfData;
	ZERO_INITIALIZE(BSDFData, bsdfData);

	bsdfData.specularOcclusion = surfaceData.specularOcclusion;
	bsdfData.normalWS = surfaceData.normalWS;
	bsdfData.perceptualRoughness = PerceptualSmoothnessToPerceptualRoughness(surfaceData.perceptualSmoothness);
	bsdfData.roughness = PerceptualRoughnessToRoughness(bsdfData.perceptualRoughness);
	bsdfData.materialId = surfaceData.materialId;

	FillMaterialIdStandardData(surfaceData.baseColor, surfaceData.specular, 0, bsdfData.roughness, surfaceData.normalWS, surfaceData.tangentWS, surfaceData.anisotropy, bsdfData);
	bsdfData.materialId = surfaceData.anisotropy > 0.0 ? MATERIALID_LIT_ANISO : bsdfData.materialId;

    FillMaterialIdSSSData(surfaceData.baseColor, surfaceData.subsurfaceProfile, surfaceData.subsurfaceRadius, surfaceData.thickness, bsdfData);

	return bsdfData;
}

float4 EncodeSplitLightingGBuffer0(SurfaceData surfaceData)
{
	return float4(surfaceData.baseColor, 1.0);
}

float4 EncodeSplitLightingGBuffer1(SurfaceData surfaceData)
{
	return float4(surfaceData.subsurfaceRadius, 1.0, 0.0, PackByte(surfaceData.subsurfaceProfile)); //TODO: Eye UI
}

//-----------------------------------------------------------------------------
// bake lighting function
//-----------------------------------------------------------------------------

// GetBakedDiffuseLigthing function compute the bake lighting + emissive color to be store in emissive buffer (Deferred case)
// In forward it must be add to the final contribution.
// This function require the 3 structure surfaceData, builtinData, bsdfData because it may require both the engine side data, and data that will not be store inside the gbuffer.
float3 GetBakedDiffuseLigthing(SurfaceData surfaceData, BuiltinData builtinData, BSDFData bsdfData, PreLightData preLightData)
{
	// Premultiply bake diffuse lighting information with DisneyDiffuse pre-integration
	return builtinData.bakeDiffuseLighting * preLightData.diffuseFGD * surfaceData.ambientOcclusion * bsdfData.diffuseColor + builtinData.emissiveColor;
}

//-----------------------------------------------------------------------------
// light transport functions
//-----------------------------------------------------------------------------

LightTransportData GetLightTransportData(SurfaceData surfaceData, BuiltinData builtinData, BSDFData bsdfData)
{
	LightTransportData lightTransportData;

	// diffuseColor for lightmapping should basically be diffuse color.
	// But rough metals (black diffuse) still scatter quite a lot of light around, so
	// we want to take some of that into account too.

	lightTransportData.diffuseColor = bsdfData.diffuseColor;
	lightTransportData.emissiveColor = builtinData.emissiveColor;

	return lightTransportData;
}

//-----------------------------------------------------------------------------
// LightLoop related function (Only include if required)
// HAS_LIGHTLOOP is define in Lighting.hlsl
//-----------------------------------------------------------------------------

#ifdef HAS_LIGHTLOOP

//-----------------------------------------------------------------------------
// BSDF share between directional light, punctual light and area light (reference)
//-----------------------------------------------------------------------------

#define BSDF BSDF_EYE


void BSDF_EYE(float3 V, float3 L, float3 positionWS, PreLightData preLightData, BSDFData bsdfData,
	out float3 diffuseLighting,
	out float3 specularLighting)
{
	// Optimized math. Ref: PBR Diffuse Lighting for GGX + Smith Microsurfaces (slide 114).
	float NdotL = saturate(dot(bsdfData.normalWS, L)); // Must have the same value without the clamp
	float NdotV = preLightData.NdotV;                  // Get the unaltered (geometric) version
	float LdotV = dot(L, V);
	float invLenLV = rsqrt(abs(2 * LdotV + 2));           // invLenLV = rcp(length(L + V))
	float NdotH = saturate((NdotL + NdotV) * invLenLV);
	float LdotH = saturate(invLenLV * LdotV + invLenLV);

	NdotV = max(NdotV, MIN_N_DOT_V);             // Use the modified (clamped) version

	float3 F = F_Schlick(bsdfData.fresnel0, LdotH);

	float Vis;
	float D;
	// TODO: this way of handling aniso may not be efficient, or maybe with material classification, need to check perf here
	// Maybe always using aniso maybe a win ?
	if (bsdfData.materialId == MATERIALID_LIT_ANISO)
	{
		float3 H = (L + V) * invLenLV;
		// For anisotropy we must not saturate these values
		float TdotH = dot(bsdfData.tangentWS, H);
		float TdotL = dot(bsdfData.tangentWS, L);
		float BdotH = dot(bsdfData.bitangentWS, H);
		float BdotL = dot(bsdfData.bitangentWS, L);

		bsdfData.roughnessT = ClampRoughnessForAnalyticalLights(bsdfData.roughnessT);
		bsdfData.roughnessB = ClampRoughnessForAnalyticalLights(bsdfData.roughnessB);

#ifdef LIT_USE_BSDF_PRE_LAMBDAV
		Vis = V_SmithJointGGXAnisoLambdaV(preLightData.TdotV, preLightData.BdotV, NdotV, TdotL, BdotL, NdotL,
			bsdfData.roughnessT, bsdfData.roughnessB, preLightData.anisoGGXLambdaV);
#else
		// TODO: Do comparison between this correct version and the one from isotropic and see if there is any visual difference
		Vis = V_SmithJointGGXAniso(preLightData.TdotV, preLightData.BdotV, NdotV, TdotL, BdotL, NdotL,
			bsdfData.roughnessT, bsdfData.roughnessB);
#endif

		D = D_GGXAniso(TdotH, BdotH, NdotH, bsdfData.roughnessT, bsdfData.roughnessB);
	}
	else
	{
		bsdfData.roughness = ClampRoughnessForAnalyticalLights(bsdfData.roughness);

#ifdef LIT_USE_BSDF_PRE_LAMBDAV
		Vis = V_SmithJointGGX(NdotL, NdotV, bsdfData.roughness, preLightData.ggxLambdaV);
#else
		Vis = V_SmithJointGGX(NdotL, NdotV, bsdfData.roughness);
#endif
		D = D_GGX(NdotH, bsdfData.roughness);
	}
	specularLighting = 10*F * (Vis * D);

#ifdef LIT_DIFFUSE_LAMBERT_BRDF
	float  diffuseTerm = Lambert();
#elif LIT_DIFFUSE_GGX_BRDF
	float3 diffuseTerm = DiffuseGGX(bsdfData.diffuseColor, NdotV, NdotL, NdotH, LdotV, bsdfData.perceptualRoughness);
#else
	float  diffuseTerm = DisneyDiffuse(NdotV, NdotL, LdotH, bsdfData.perceptualRoughness);
#endif

	diffuseLighting = bsdfData.diffuseColor * diffuseTerm;
}

#endif // #ifdef HAS_LIGHTLOOP

#include "../LightEvaluationShare2.hlsl"