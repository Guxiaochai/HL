#ifndef CUSTOM_CAMERA_RENDERER_PASSES_INCLUDED
#define CUSTOM_CAMERA_RENDERER_PASSES_INCLUDED

TEXTURE2D(_SourceTexture);

struct Varyings{
    float4 positionCS_SS : SV_POSITION;
    float2 screenUV : VAR_SCREEN_UV;
	float2 baseUV : TEXCOORD0;
	float2 uv_depth : TEXCOORD1;
	float4 interpolatedRay : TEXCOORD2;
};

Varyings DefaultPassVertex(uint vertexID : SV_VertexID){
    Varyings output;
    output.positionCS_SS = float4(
        vertexID <= 1 ? -1.0 : 3.0,
        vertexID == 1 ? 3.0 : -1.0,
        0.0, 1.0
    );
    output.screenUV = float2(
		vertexID <= 1 ? 0.0 : 2.0,
		vertexID == 1 ? 2.0 : 0.0
	);

    if(_ProjectionParams.x < 0.0){
        output.screenUV.y = 1.0 - output.screenUV.y;
    }
	return output;
}

float4 CopyPassFragment (Varyings input) : SV_TARGET {
	return SAMPLE_TEXTURE2D_LOD(_SourceTexture, sampler_linear_clamp, input.screenUV, 0);
    //return float4(1.0, 0.0, 0.0, 1.0);
}

#endif