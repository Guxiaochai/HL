using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

public partial class CustomRenderPipeline : RenderPipeline
{
    readonly RenderGraph renderGraph = new("Emmer Custom SRP Render Graph");

    CameraRenderer renderer;

    CameraBufferSettings cameraBufferSettings;

    bool useDynamicBatching, useGPUInstancing, useLightsPerObject;
    ShadowSettings shadowSettings;

    PostFXSettings postFXSettings;

    int colorLUTResolution;

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach(Camera camera in cameras)
        {
            renderer.Render(renderGraph, context, camera, cameraBufferSettings, useDynamicBatching, useGPUInstancing, 
                            useLightsPerObject, shadowSettings, postFXSettings, colorLUTResolution);
        }
        renderGraph.EndFrame();
    }

    public CustomRenderPipeline(CameraBufferSettings cameraBufferSettings, bool useDynamicBatching, bool useGPUInstancing,
                                bool useSRPBatcher, bool useLightsPerObject, ShadowSettings shadowSettings,
                                PostFXSettings postFXSettings, int colorLUTResolution, Shader cameraRendererShader)
    {
        this.cameraBufferSettings = cameraBufferSettings;
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
