#ifndef CUSTOM_POST_FX_PASSES_INCLUDED
#define CUSTOM_POST_FX_PASSES_INCLUDED

TEXTURE2D(_PostFXSource);
SAMPLER(sampler_linear_clamp);

struct Varyings{
    float4 positionCS : SV_POSITION;
    float2 screenUV : VAR_SCREEN_UV;
};

float4 GetSource(float2 screenUV){
    return SAMPLE_TEXTURE2D_LOD(_PostFXSource, sampler_linear_clamp, screenUV, 0);
}

Varyings DefaultPassVertex(uint vertexID : SV_VertexID){
    Varyings output;
    output.positionCS = float4(
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

float4 CopyPassFragment(Varyings input) : SV_TARGET{
    return GetSource(input.screenUV);
}

#endif