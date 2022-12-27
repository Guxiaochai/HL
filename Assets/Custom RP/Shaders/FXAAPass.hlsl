#ifndef CUSTOM_FXAA_PASS_INCLUDED
#define CUSTOM_FXAA_PASS_INCLUDED

float4 FXAAPassFragment (Varyings input) : SV_TARGET {
	return GetSource(input.screenUV);
}

#endif