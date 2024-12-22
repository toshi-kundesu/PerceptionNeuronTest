#ifndef RETURNRED_INCLUDED
#define RETURNRED_INCLUDED

void ReturnRed_float(out half3 color)
{
#ifdef SHADERGRAPH_PREVIEW
    // In Shader Graph Preview we will assume a default light direction and white color
    
    color = half3(1, 0, 0);
#else

    // // Universal Render Pipeline
    // #if defined(UNIVERSAL_LIGHTING_INCLUDED)
    
    //     // GetMainLight defined in Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl
    //     Light mainLight = GetMainLight();
    //     direction = -mainLight.direction;
    //     color = mainLight.color;
    //     distanceAttenuation = mainLight.distanceAttenuation;
    //     shadowAttenuation = mainLight.shadowAttenuation;
    
    // #elif defined(HD_LIGHTING_INCLUDED) 
    //     // ToDo: make it work for HDRP (check define above)
    //     // Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightDefinition.cs.hlsl
    //     // if (_DirectionalLightCount > 0)
    //     // {
    //         DirectionalLightData light = _DirectionalLightDatas[0];
    //         lightDir = -light.forward.xyz;
    //         // direction = lightDir;
    //         direction = half3(-0.3, -0.8, 0.6);
    //             distanceAttenuation = 1.0;
    //             shadowAttenuation = 1.0;
    //         color = light.color;
    //     // }
    //     //     ......
    color = half3(1, 0, 0);
        
    // #endif

#endif
}
#endif // RETURNRED_INCLUDED

