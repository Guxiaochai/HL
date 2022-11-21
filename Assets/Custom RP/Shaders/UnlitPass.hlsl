#ifndef CUSTOM_UNLIT_PASS_INCLUDED
#define CUSTOM_UNLIT_PASS_INCLUDED

struct Attributes{
    float3 positionOS : POSITION;
    float4 color : COLOR;
    float2 baseUV : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings{
    float4 positionCS : SV_POSITION;
    #if defined(_VERTEX_COLORS)
        float4 color : VAR_COLOR;
    #endif
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
    #if defined(_VERTEX_COLORS)
        output.color = input.color;
    #endif
    return output;
}

float4 UnlitPassFragment (Varyings input) : SV_TARGET{
    UNITY_SETUP_INSTANCE_ID(input);
    InputConfig config = GetInputConfig(input.baseUV);
    #if defined(_VERTEX_COLORS)
        config.color = input.color;
    #endif
    float4 base = GetBase(config);
#if defined(_CLIPPING)
    clip(base.a - GetCutoff(config)); // discard the fragment if the parameter is less or equal than zero
#endif
    return float4(base.rgb, GetFinalAlpha(base.a));
}

#endif