#ifndef CUSTOM_SURFACE_INCLUDED
#define CUSTOM_SURFACE_INCLUDED

struct Surface{
    float3 position;
    float3 normal;
    float3 viewDirection;
    float3 color;
    float alpha;
    float smoothness;
    float metallic;
    float depth;
    float dither;
};

#endif