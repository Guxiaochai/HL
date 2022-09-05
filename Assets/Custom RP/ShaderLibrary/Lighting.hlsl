#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

float3 IncomingLight(Surface surface, Light light){
    return saturate(dot(surface.normal, light.direction) * light.attenuation)  * light.color;
}

float3 GetLighting(Surface surface, BRDF brdf, Light light){
    return IncomingLight(surface, light) * DirectBRDF(surface, brdf, light);
}

float3 GetLighting(Surface surfaceWS, BRDF brdf, GI gi){
    ShadowData shadowData = GetShadowData(surfaceWS);
	shadowData.shadowMask = gi.shadowMask;
    //float3 color = gi.diffuse * brdf.diffuse;
    float3 color = IndirectBRDF(surfaceWS, brdf, gi.diffuse, gi.specular);
    for(int i = 0; i < GetDirectionalLightCount(); i++){
        Light light = GetDirectionalLight(i, surfaceWS, shadowData);
        color += GetLighting(surfaceWS, brdf, light);
    }

    for(int j = 0; j < GetOtherLightCount(); j++){
        Light light = GetOtherLight(j, surfaceWS, shadowData);
        color += GetLighting(surfaceWS, brdf, light);
    }
    return color;
}

#endif