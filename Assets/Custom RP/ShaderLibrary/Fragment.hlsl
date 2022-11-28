#ifndef FRAGMENT_INCLUDED
#define FRAGMENT_INCLUDED

struct Fragment {
	float2 positionSS;
};

Fragment GetFragment (float4 positionSS) {
	Fragment f;
	f.positionSS = positionSS.xy;
	return f;
}

#endif