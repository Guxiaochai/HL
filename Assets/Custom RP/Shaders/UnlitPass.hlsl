#ifndef CUSTOM_UNLIT_PASS_INCLUDED
#define CUSTOM_UNLIT_PASS_INCLUDED

struct Attributes{
    float3 positionOS : POSITION;
    float2 baseUV : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings{
    float4 positionCS : SV_POSITION;
    float2 baseUV : VAR_BASE_UV;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

Varyings UnlitPassVertex (Attributes input) {
    Varyings output;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    float3 positionWS = mul(UNITY_MATRIX_M, float4(input.positionOS, 1.0));
    output.positionCS = TransformWorldToHClip(positionWS);
    output.baseUV = TransformBaseUV(input.baseUV);
    return output;
}

float4 UnlitPassFragment (Varyings input) : SV_TARGET{
    UNITY_SETUP_INSTANCE_ID(input);
    float4 base = GetBase(input.baseUV);
#if defined(_CLIPPING)
    clip(base.a - GetCutoff(input.baseUV));// discard the fragment if the parameter is less or equal than zero
#endif
    return base;
}

#endif