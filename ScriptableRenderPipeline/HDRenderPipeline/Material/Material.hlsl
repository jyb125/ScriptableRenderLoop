#ifndef UNITY_MATERIAL_INCLUDED
#define UNITY_MATERIAL_INCLUDED

#include "../../Core/ShaderLibrary/Color.hlsl"
#include "../../Core/ShaderLibrary/Packing.hlsl"
#include "../../Core/ShaderLibrary/BSDF.hlsl"
#include "../../Core/ShaderLibrary/Debug.hlsl"
#include "../../Core/ShaderLibrary/GeometricTools.hlsl"
#include "../../Core/ShaderLibrary/CommonMaterial.hlsl"
#include "../../Core/ShaderLibrary/EntityLighting.hlsl"
#include "../../Core/ShaderLibrary/ImageBasedLighting.hlsl"
#include "../Sky/AtmosphericScattering/AtmosphericScattering.hlsl"

//-----------------------------------------------------------------------------
// Blending
//-----------------------------------------------------------------------------
// This should match the possible blending modes in any material .shader file (lit/layeredlit/unlit etc)
#if defined(_BLENDMODE_ALPHA) || defined(_BLENDMODE_ADD) || defined(_BLENDMODE_MULTIPLY) || defined(_BLENDMODE_PRE_MULTIPLY)
#define SURFACE_TYPE_TRANSPARENT
#else
#define SURFACE_TYPE_OPAQUE
#endif

//-----------------------------------------------------------------------------
// Fog sampling function for materials
//-----------------------------------------------------------------------------

// Used for transparent object. input color is color + alpha of the original transparent pixel.
float4 EvaluateAtmosphericScattering(PositionInputs posInput, float4 inputColor)
{
    float4 result = inputColor;

#if defined(SURFACE_TYPE_TRANSPARENT) && defined(_ENABLE_FOG)
    float4 fog = EvaluateAtmosphericScattering(posInput);

#if defined(_BLENDMODE_ALPHA)
    // Regular alpha blend only need a lerp to work
    result.rgb = lerp(result.rgb, fog.rgb, fog.a);
#elif defined(_BLENDMODE_ADD)
    // For additive, we just need to fade to black with fog density (black + background == background color == fog color)
    result.rgb = result.rgb * (1.0 - fog.a);
#elif defined(_BLENDMODE_MULTIPLY) 
    // For multiplicative, we just need to fade to white with fog density (white * background == background color == fog color)
    result.rgb = lerp(result.rgb, float3(1.0, 1.0, 1.0), fog.a);
#elif defined(_BLENDMODE_PRE_MULTIPLY)
    // For Pre-Multiplied Alpha Blend, we need to multiply fog color by src alpha to match regular alpha blending formula.
    result.rgb = lerp(result.rgb, fog.rgb * result.a, fog.a);
#endif

#else
    // Evaluation of fog for opaque objects is currently done in a full screen pass independent from any material parameters.
    // but this funtction is called in generic forward shader code so we need it to be neutral in this case.
#endif

    return result;
}


//-----------------------------------------------------------------------------
// BuiltinData
//-----------------------------------------------------------------------------

#include "Builtin/BuiltinData.hlsl"

//-----------------------------------------------------------------------------
// Material definition
//-----------------------------------------------------------------------------

// Here we include all the different lighting model supported by the renderloop based on define done in .shader
// Only one deferred layout is allowed for a HDRenderPipeline, this will be detect by the redefinition of GBUFFERMATERIAL_COUNT
// If GBUFFERMATERIAL_COUNT is define two time, the shaders will not compile
#ifdef UNITY_MATERIAL_LIT
#include "Lit/Lit.hlsl"
#elif defined(UNITY_MATERIAL_UNLIT)
#include "Unlit/Unlit.hlsl"
#elif defined(UNITY_MATERIAL_IRIDESCENCE)
//#include "Iridescence/Iridescence.hlsl"
#endif

//-----------------------------------------------------------------------------
// Define for GBuffer management
//-----------------------------------------------------------------------------

#ifdef GBUFFERMATERIAL_COUNT

#if GBUFFERMATERIAL_COUNT == 2

#define OUTPUT_GBUFFER(NAME)                            \
        out GBufferType0 MERGE_NAME(NAME, 0) : SV_Target0,    \
        out GBufferType1 MERGE_NAME(NAME, 1) : SV_Target1

#define DECLARE_GBUFFER_TEXTURE(NAME)   \
        TEXTURE2D(MERGE_NAME(NAME, 0));  \
        TEXTURE2D(MERGE_NAME(NAME, 1));

#define FETCH_GBUFFER(NAME, TEX, unCoord2)                                        \
        GBufferType0 MERGE_NAME(NAME, 0) = LOAD_TEXTURE2D(MERGE_NAME(TEX, 0), unCoord2); \
        GBufferType1 MERGE_NAME(NAME, 1) = LOAD_TEXTURE2D(MERGE_NAME(TEX, 1), unCoord2);

#define ENCODE_INTO_GBUFFER(SURFACE_DATA, BAKE_DIFFUSE_LIGHTING, NAME) EncodeIntoGBuffer(SURFACE_DATA, BAKE_DIFFUSE_LIGHTING, MERGE_NAME(NAME,0), MERGE_NAME(NAME,1))
#define DECODE_FROM_GBUFFER(NAME, FEATURE_FLAGS, BSDF_DATA, BAKE_DIFFUSE_LIGHTING) DecodeFromGBuffer(MERGE_NAME(NAME,0), MERGE_NAME(NAME,1), FEATURE_FLAGS, BSDF_DATA, BAKE_DIFFUSE_LIGHTING)
#define MATERIAL_FEATURE_FLAGS_FROM_GBUFFER(NAME) MaterialFeatureFlagsFromGBuffer(MERGE_NAME(NAME,0), MERGE_NAME(NAME,1))

#if SHADEROPTIONS_VELOCITY_IN_GBUFFER
#define OUTPUT_GBUFFER_VELOCITY(NAME) ,out float4 NAME : SV_Target2
#endif

#elif GBUFFERMATERIAL_COUNT == 3

#define OUTPUT_GBUFFER(NAME)                            \
        out GBufferType0 MERGE_NAME(NAME, 0) : SV_Target0,    \
        out GBufferType1 MERGE_NAME(NAME, 1) : SV_Target1,    \
        out GBufferType2 MERGE_NAME(NAME, 2) : SV_Target2

#define DECLARE_GBUFFER_TEXTURE(NAME)   \
        TEXTURE2D(MERGE_NAME(NAME, 0));  \
        TEXTURE2D(MERGE_NAME(NAME, 1));  \
        TEXTURE2D(MERGE_NAME(NAME, 2));

#define FETCH_GBUFFER(NAME, TEX, unCoord2)                                        \
        GBufferType0 MERGE_NAME(NAME, 0) = LOAD_TEXTURE2D(MERGE_NAME(TEX, 0), unCoord2); \
        GBufferType1 MERGE_NAME(NAME, 1) = LOAD_TEXTURE2D(MERGE_NAME(TEX, 1), unCoord2); \
        GBufferType2 MERGE_NAME(NAME, 2) = LOAD_TEXTURE2D(MERGE_NAME(TEX, 2), unCoord2);

#define ENCODE_INTO_GBUFFER(SURFACE_DATA, BAKE_DIFFUSE_LIGHTING, NAME) EncodeIntoGBuffer(SURFACE_DATA, BAKE_DIFFUSE_LIGHTING, MERGE_NAME(NAME,0), MERGE_NAME(NAME,1), MERGE_NAME(NAME,2))
#define DECODE_FROM_GBUFFER(NAME, FEATURE_FLAGS, BSDF_DATA, BAKE_DIFFUSE_LIGHTING) DecodeFromGBuffer(MERGE_NAME(NAME,0), MERGE_NAME(NAME,1), MERGE_NAME(NAME,2), FEATURE_FLAGS, BSDF_DATA, BAKE_DIFFUSE_LIGHTING)
#define MATERIAL_FEATURE_FLAGS_FROM_GBUFFER(NAME) MaterialFeatureFlagsFromGBuffer(MERGE_NAME(NAME,0), MERGE_NAME(NAME,1), MERGE_NAME(NAME,2))


#if SHADEROPTIONS_VELOCITY_IN_GBUFFER
#define OUTPUT_GBUFFER_VELOCITY(NAME) ,out float4 NAME : SV_Target3
#endif

#elif GBUFFERMATERIAL_COUNT == 4

#define OUTPUT_GBUFFER(NAME)                            \
        out GBufferType0 MERGE_NAME(NAME, 0) : SV_Target0,    \
        out GBufferType1 MERGE_NAME(NAME, 1) : SV_Target1,    \
        out GBufferType2 MERGE_NAME(NAME, 2) : SV_Target2,    \
        out GBufferType3 MERGE_NAME(NAME, 3) : SV_Target3

#define DECLARE_GBUFFER_TEXTURE(NAME)   \
        TEXTURE2D(MERGE_NAME(NAME, 0));  \
        TEXTURE2D(MERGE_NAME(NAME, 1));  \
        TEXTURE2D(MERGE_NAME(NAME, 2));  \
        TEXTURE2D(MERGE_NAME(NAME, 3));

#define FETCH_GBUFFER(NAME, TEX, unCoord2)                                        \
        GBufferType0 MERGE_NAME(NAME, 0) = LOAD_TEXTURE2D(MERGE_NAME(TEX, 0), unCoord2); \
        GBufferType1 MERGE_NAME(NAME, 1) = LOAD_TEXTURE2D(MERGE_NAME(TEX, 1), unCoord2); \
        GBufferType2 MERGE_NAME(NAME, 2) = LOAD_TEXTURE2D(MERGE_NAME(TEX, 2), unCoord2); \
        GBufferType3 MERGE_NAME(NAME, 3) = LOAD_TEXTURE2D(MERGE_NAME(TEX, 3), unCoord2);

#define ENCODE_INTO_GBUFFER(SURFACE_DATA, BAKE_DIFFUSE_LIGHTING, NAME) EncodeIntoGBuffer(SURFACE_DATA, BAKE_DIFFUSE_LIGHTING, MERGE_NAME(NAME, 0), MERGE_NAME(NAME, 1), MERGE_NAME(NAME, 2), MERGE_NAME(NAME, 3))
#define DECODE_FROM_GBUFFER(NAME, FEATURE_FLAGS, BSDF_DATA, BAKE_DIFFUSE_LIGHTING) DecodeFromGBuffer(MERGE_NAME(NAME, 0), MERGE_NAME(NAME, 1), MERGE_NAME(NAME, 2), MERGE_NAME(NAME, 3), FEATURE_FLAGS, BSDF_DATA, BAKE_DIFFUSE_LIGHTING)
#define MATERIAL_FEATURE_FLAGS_FROM_GBUFFER(NAME) MaterialFeatureFlagsFromGBuffer(MERGE_NAME(NAME, 0), MERGE_NAME(NAME, 1), MERGE_NAME(NAME, 2), MERGE_NAME(NAME, 3))

#if SHADEROPTIONS_VELOCITY_IN_GBUFFER
#define OUTPUT_GBUFFER_VELOCITY(NAME) ,out float4 NAME : SV_Target4
#endif

#elif GBUFFERMATERIAL_COUNT == 5

#define OUTPUT_GBUFFER(NAME)                            \
        out GBufferType0 MERGE_NAME(NAME, 0) : SV_Target0,    \
        out GBufferType1 MERGE_NAME(NAME, 1) : SV_Target1,    \
        out GBufferType2 MERGE_NAME(NAME, 2) : SV_Target2,    \
        out GBufferType3 MERGE_NAME(NAME, 3) : SV_Target3,    \
        out GBufferType4 MERGE_NAME(NAME, 4) : SV_Target4

#define DECLARE_GBUFFER_TEXTURE(NAME)   \
        TEXTURE2D(MERGE_NAME(NAME, 0));  \
        TEXTURE2D(MERGE_NAME(NAME, 1));  \
        TEXTURE2D(MERGE_NAME(NAME, 2));  \
        TEXTURE2D(MERGE_NAME(NAME, 3));  \
        TEXTURE2D(MERGE_NAME(NAME, 4));

#define FETCH_GBUFFER(NAME, TEX, unCoord2)                                        \
        GBufferType0 MERGE_NAME(NAME, 0) = LOAD_TEXTURE2D(MERGE_NAME(TEX, 0), unCoord2); \
        GBufferType1 MERGE_NAME(NAME, 1) = LOAD_TEXTURE2D(MERGE_NAME(TEX, 1), unCoord2); \
        GBufferType2 MERGE_NAME(NAME, 2) = LOAD_TEXTURE2D(MERGE_NAME(TEX, 2), unCoord2); \
        GBufferType3 MERGE_NAME(NAME, 3) = LOAD_TEXTURE2D(MERGE_NAME(TEX, 3), unCoord2); \
        GBufferType4 MERGE_NAME(NAME, 4) = LOAD_TEXTURE2D(MERGE_NAME(TEX, 4), unCoord2);

#define ENCODE_INTO_GBUFFER(SURFACE_DATA, BAKE_DIFFUSE_LIGHTING, NAME) EncodeIntoGBuffer(SURFACE_DATA, BAKE_DIFFUSE_LIGHTING, MERGE_NAME(NAME, 0), MERGE_NAME(NAME, 1), MERGE_NAME(NAME, 2), MERGE_NAME(NAME, 3), MERGE_NAME(NAME, 4))
#define DECODE_FROM_GBUFFER(NAME, FEATURE_FLAGS, BSDF_DATA, BAKE_DIFFUSE_LIGHTING) DecodeFromGBuffer(MERGE_NAME(NAME, 0), MERGE_NAME(NAME, 1), MERGE_NAME(NAME, 2), MERGE_NAME(NAME, 3), MERGE_NAME(NAME, 4), FEATURE_FLAGS, BSDF_DATA, BAKE_DIFFUSE_LIGHTING)
#define MATERIAL_FEATURE_FLAGS_FROM_GBUFFER(NAME) MaterialFeatureFlagsFromGBuffer(MERGE_NAME(NAME, 0), MERGE_NAME(NAME, 1), MERGE_NAME(NAME, 2), MERGE_NAME(NAME, 3), MERGE_NAME(NAME, 4))

#if SHADEROPTIONS_VELOCITY_IN_GBUFFER
#define OUTPUT_GBUFFER_VELOCITY(NAME) ,out float4 NAME : SV_Target5
#endif

#endif

#if SHADEROPTIONS_VELOCITY_IN_GBUFFER
#define ENCODE_VELOCITY_INTO_GBUFFER(VELOCITY, NAME) EncodeVelocity(VELOCITY, NAME)
#else
#define OUTPUT_GBUFFER_VELOCITY(NAME)
#define ENCODE_VELOCITY_INTO_GBUFFER(VELOCITY, NAME)
#endif

#endif // #ifdef GBUFFERMATERIAL_COUNT

#endif // UNITY_MATERIAL_INCLUDED
