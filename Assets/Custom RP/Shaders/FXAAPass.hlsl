#ifndef CUSTOM_FXAA_PASS_INCLUDED
#define CUSTOM_FXAA_PASS_INCLUDED

float4 _FXAAConfig;

struct LumaNeighborhood{
	float m, n, e, s, w, ne, se, sw, nw;
	float highest, lowest, range;
};

struct FXAAEdge
{
	bool isHorizontal;
	float pixelStep;
};

bool CanSkipFXAA(LumaNeighborhood luma){
	return luma.range < max(_FXAAConfig.x, _FXAAConfig.y * luma.highest);
}

float GetLuma(float2 uv, float uOffset = 0.0, float vOffset = 0.0){

	uv += float2(uOffset, vOffset) * GetSourceTexelSize().xy;
	#if defined(FXAA_ALPHA_CONTAINS_LUMA)
		return GetSource(uv).a;
	#else
		return GetSource(uv).g;
	#endif
}

LumaNeighborhood GetLumaNeighborhood(float2 uv){
	LumaNeighborhood luma;
	luma.m = GetLuma(uv);
	luma.n = GetLuma(uv, 0.0, 1.0);
	luma.e = GetLuma(uv, 1.0, 0.0);
	luma.s = GetLuma(uv, 0.0, -1.0);
	luma.w = GetLuma(uv, -1.0, 0.0);
	luma.ne = GetLuma(uv, 1.0, 1.0);
	luma.se = GetLuma(uv, 1.0, -1.0);
	luma.sw = GetLuma(uv, -1.0, -1.0);
	luma.nw = GetLuma(uv, -1.0, 1.0);
	luma.highest = max(max(max(max(luma.m, luma.n), luma.e), luma.s), luma.w);
	luma.lowest = min(min(min(min(luma.m, luma.n), luma.e), luma.s), luma.w);
	luma.range = luma.highest - luma.lowest;
	return luma;
}

float GetSubpixelBlendFactor(LumaNeighborhood luma){
	float filter = 2.0 * (luma.n + luma.e + luma.s + luma.w);
	filter += luma.ne + luma.nw + luma.se + luma.sw;
	filter *= 1.0 / 12.0;
	filter = saturate(filter / luma.range);
	filter = smoothstep(0, 1, filter);
	return filter * filter;
}

bool IsHorizontalEdge(LumaNeighborhood luma)
{
	float horizontal = 2.0 * abs(luma.n + luma.s - 2.0 * luma.m) +
			abs(luma.nw + luma.sw - 2.0 * luma.w) +
			abs(luma.ne + luma.se - 2.0 * luma.e);
	float vertical = abs(luma.e + luma.w - 2.0 * luma.m) +
			abs(luma.nw + luma.ne - 2.0 * luma.n) +
			abs(luma.sw + luma.se - 2.0 * luma.s);
	return horizontal >= vertical;
}

FXAAEdge GetFXAAEdge(LumaNeighborhood luma)
{
	FXAAEdge edge;
	edge.isHorizontal = IsHorizontalEdge(luma);
	if (edge.isHorizontal)
	{
		edge.pixelStep = GetSourceTexelSize().y;
	}
	else
	{
		edge.pixelStep = GetSourceTexelSize().x;
	}
	return edge;
}

// 这个函数用作测试
float testFuc(LumaNeighborhood luma)
{
	float filter = luma.n + luma.e + luma.s + luma.w;
	filter *= 1.0 / 4.0;
	filter = abs(filter - luma.m);
	filter = filter / luma.range;
	filter = smoothstep(0, 1, filter);
	return filter * filter;
}

float4 FXAAPassFragment (Varyings input) : SV_TARGET {
	LumaNeighborhood luma = GetLumaNeighborhood(input.screenUV);
	if(CanSkipFXAA(luma)){
		return 0.0;
	}

	FXAAEdge edge = GetFXAAEdge(luma);
	//return luma.range;
	//return GetSubpixelBlendFactor(luma);
	//return testFuc(luma);
	return edge.isHorizontal ? float4(1.0, 0.0, 0.0, 0.0) : 1.0;
}

#endif