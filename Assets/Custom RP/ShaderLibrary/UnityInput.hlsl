#ifndef CUSTOM_UNITY_INPUT_INCLUDED
#define CUSTOM_UNITY_INPUT_INCLUDED

CBUFFER_START(UnityPerDraw)
    float4x4 unity_ObjectToWorld;
    float4x4 unity_WorldToObject;
	
    float4 unity_LODFade;
    real4 unity_WorldTransformParams;

    float4 unity_RenderingLayer;
    real4 unity_LightData;
    real4 unity_LightIndices[2];
    float4 unity_ProbesOcclusion;
    float4 unity_SpecCube0_HDR;
    float3 _WorldSpaceCameraPos;
    // to sampler lightmap
    float4 unity_LightmapST;
    float4 unity_DynamicLightmapST;
    // to sample light probe
    float4 unity_SHAr;
	float4 unity_SHAg;
	float4 unity_SHAb;
	float4 unity_SHBr;
	float4 unity_SHBg;
	float4 unity_SHBb;
	float4 unity_SHC;
    // to sampler LPPV
    float4 unity_ProbeVolumeParams;
    float4x4 unity_ProbeVolumeWorldToObject;
    float4 unity_ProbeVolumeSizeInv;
    float4 unity_ProbeVolumeMin;

	float4x4 unity_MatrixPreviousM;
	float4x4 unity_MatrixPreviousMI;
	float4x4 unity_MatrixInvV;
CBUFFER_END

float4x4 unity_MatrixVP;
float4x4 unity_MatrixV;
float4x4 glstate_matrix_projection;
float4 unity_OrthoParams;
float4 _ProjectionParams;
float4 _ScreenParams;
float4 _ZBufferParams;

#endif