using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class CustomRenderPipeline : RenderPipeline
{
    CameraRenderer renderer;

    bool allowHDR, useDynamicBatching, useGPUInstancing, useLightsPerObject;
    ShadowSettings shadowSettings;

    PostFXSettings postFXSettings;

    int colorLUTResolution;

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach(Camera camera in cameras)
        {
            renderer.Render(context, camera, allowHDR, useDynamicBatching, useGPUInstancing, 
                            useLightsPerObject, shadowSettings, postFXSettings, colorLUTResolution);
        }
       
    }

    public CustomRenderPipeline(bool allowHDR, bool useDynamicBatching, bool useGPUInstancing,
                                bool useSRPBatcher, bool useLightsPerObject, ShadowSettings shadowSettings,
                                PostFXSettings postFXSettings, int colorLUTResolution, Shader cameraRendererShader)
    {
        this.allowHDR = allowHDR;
        this.useDynamicBatching = useDynamicBatching;
        this.useGPUInstancing = useGPUInstancing;
        this.shadowSettings = shadowSettings;
        this.postFXSettings = postFXSettings;
        this.colorLUTResolution = colorLUTResolution;
        this.useLightsPerObject = useLightsPerObject;
        GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
        GraphicsSettings.lightsUseLinearIntensity = true;
        renderer = new CameraRenderer(cameraRendererShader);
        InitializeForEditor();
    }

}
