using UnityEngine;
using UnityEngine.Rendering;
using System;
using static PostFXSettings;

public partial class PostFXStack
{
    enum Pass
    {
        //BloomCombine,
        BloomAdd,
        BloomHorizontal,
        BloomPrefilter,
        BloomPrefilterFireflies,
        BloomScatter,
        BloomScatterFinal,
        BloomVertical,
        Copy,
        ColorGradingNone,
        ColorGradingACES,
        ColorGradingNeutral,
        ColorGradingReinhard,
        ScannerEffect,
        ApplyColorGrading,
        FinalRescale,
        FXAA
    }

    Vector2Int bufferSize;

    bool useHDR;

    const int maxBloomPyramidLevels = 16;
    int bloomPyramidId;

    CameraBufferSettings.BicubicRescalingMode bicubicRescaling;

    int
        bloomBicubicUpsamplingId = Shader.PropertyToID("_BloomBicubicUpsampling"),
        copyBicubicId = Shader.PropertyToID("_CopyBicubic"),
        colorGradingResultId = Shader.PropertyToID("_ColorGradingResult"),
        finalResultId = Shader.PropertyToID("_FinalResult"),
        bloomPrefilterId = Shader.PropertyToID("_BloomPrefilter"),
        bloomThresholdId = Shader.PropertyToID("_BloomThreshold"),
        bloomIntensityId = Shader.PropertyToID("_BloomIntensity"),
        bloomResultId = Shader.PropertyToID("_BloomResult"),
        colorAdjustmentsId = Shader.PropertyToID("_ColorAdjustments"),
        colorFilterId = Shader.PropertyToID("_ColorFilter"),
        whiteBalanceId = Shader.PropertyToID("_WhiteBalance"),
        splitToningShadowsId = Shader.PropertyToID("_SplitToningShadows"),
        splitToningHighlightsId = Shader.PropertyToID("_SplitToningHighlights"),
        channelMixerRedId = Shader.PropertyToID("_ChannelMixerRed"),
        channelMixerGreenId = Shader.PropertyToID("_ChannelMixerGreen"),
        channelMixerBlueId = Shader.PropertyToID("_ChannelMixerBlue"),
        smhShadowsId = Shader.PropertyToID("_SMHShadows"),
        smhMidtonesId = Shader.PropertyToID("_SMHMidtones"),
        smhHighlightsId = Shader.PropertyToID("_SMHHighlights"),
        smhRangeId = Shader.PropertyToID("_SMHRange"),
        fxSourceId = Shader.PropertyToID("_PostFXSource"),
        fxSource2Id = Shader.PropertyToID("_PostFXSource2"),
        colorGradingLUTId = Shader.PropertyToID("_ColorGradingLUT"),
        colorGradingLUTParametersId = Shader.PropertyToID("_ColorGradingLUTParameters"),
        colorGradingLUTInLogId = Shader.PropertyToID("_ColorGradingLUTInLogC"),
        seScanDistanceId = Shader.PropertyToID("_SEScanDistance"),
        seScanWidthId = Shader.PropertyToID("_SEScanWidth"),
        seLeadingEdgeSharpnessId = Shader.PropertyToID("_SELeadingEdgeSharpness"),
        seLeadingEdgeColorId = Shader.PropertyToID("_SELeadingEdgeColor"),
        seMidColorId = Shader.PropertyToID("_SEMidColor"),
        seTrailColorId = Shader.PropertyToID("_SETrailColor"),
        seHorizontalBarColorId = Shader.PropertyToID("_SEHorizontalBarColor"),
        finalSrcBlendId = Shader.PropertyToID("_FinalSrcBlend"),
        finalDstBlendId = Shader.PropertyToID("_FinalDstBlend");

    const string bufferName = "Emmer Post FX";

    CameraSettings.FinalBlendMode finalBlendMode;

    CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };

    ScriptableRenderContext context;

    Camera camera;

    PostFXSettings settings;

    CameraBufferSettings.FXAA fxaa;

    int colorLUTResolution;

    public bool IsActive => settings != null;
    //public bool IsActive => false;

    public PostFXStack()
    {
        bloomPyramidId = Shader.PropertyToID("_BloomPyramid0");
        for(int i = 0; i < maxBloomPyramidLevels * 2; i++)
        {
            Shader.PropertyToID("_BloomPyramid" + i);
        }
    }

    public void Setup(ScriptableRenderContext context, Camera camera, Vector2Int bufferSize, PostFXSettings settings, 
                      bool useHDR, int colorLUTResolution, CameraSettings.FinalBlendMode finalBlendMode, CameraBufferSettings.BicubicRescalingMode bicubicRescaling,
                      CameraBufferSettings.FXAA fxaa)
    {
        this.useHDR = useHDR;
        this.colorLUTResolution = colorLUTResolution;
        this.bufferSize = bufferSize;
        this.context = context;
        this.camera = camera;
        this.settings = camera.cameraType <= CameraType.SceneView ? settings : null;
        this.finalBlendMode = finalBlendMode;
        this.bicubicRescaling = bicubicRescaling;
        this.fxaa = fxaa;
        ApplySceneViewState();
    }

    public void Render(int sourceId)
    {
        if (DoBloom(sourceId))
        {
            DoFinal(bloomResultId);
            //DoScannerEffect(bloomResultId);
            buffer.ReleaseTemporaryRT(bloomResultId);
        }
        else
        {
            DoFinal(sourceId);
            //DoScannerEffect(sourceId);
        }
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    void Draw(RenderTargetIdentifier from, RenderTargetIdentifier to, Pass pass)
    {
        buffer.SetGlobalTexture(fxSourceId, from);
        buffer.SetRenderTarget(to, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
        buffer.DrawProcedural(Matrix4x4.identity, settings.Material, (int)pass, MeshTopology.Triangles, 3);
    }

    void DrawFinal(RenderTargetIdentifier from, Pass pass)
    {
        buffer.SetGlobalFloat(finalSrcBlendId, (int)finalBlendMode.source);
        buffer.SetGlobalFloat(finalDstBlendId, (int)finalBlendMode.destination);

        buffer.SetGlobalTexture(fxSourceId, from);
        buffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget, 
                               finalBlendMode.destination == BlendMode.Zero ? RenderBufferLoadAction.DontCare : RenderBufferLoadAction.Load, 
                               RenderBufferStoreAction.Store);
        buffer.SetViewport(camera.pixelRect);
        buffer.DrawProcedural(Matrix4x4.identity, settings.Material, (int)pass, MeshTopology.Triangles, 3);
    }

    bool DoBloom(int sourceId)
    {
        //buffer.BeginSample("Bloom");
        PostFXSettings.BloomSettings bloom = settings.Bloom;
        int width, height;
        if (bloom.ignoreRenderScale)
        {
            width = camera.pixelWidth / 2;
            height = camera.pixelHeight / 2;
        }
        else
        {
            width = bufferSize.x / 2;
            height = bufferSize.y / 2;
        }
        if (bloom.maxIterations == 0 || bloom.intensity <= 0f ||
            height < bloom.downscaleLimit * 2 || width < bloom.downscaleLimit * 2)
        {
            return false;
        }

        buffer.BeginSample("Bloom");
        Vector4 threshold;
        threshold.x = Mathf.GammaToLinearSpace(bloom.threshold);
        threshold.y = threshold.x * bloom.thresholdKnee;
        threshold.z = 2f * threshold.y;
        threshold.w = 0.25f / (threshold.y + 0.00001f);
        threshold.y -= threshold.x;
        buffer.SetGlobalVector(bloomThresholdId, threshold);

        RenderTextureFormat format = useHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
        buffer.GetTemporaryRT(
            bloomPrefilterId, width, height, 0, FilterMode.Bilinear, format
        );
        Draw(sourceId, bloomPrefilterId, bloom.fadeFireflies ? Pass.BloomPrefilterFireflies : Pass.BloomPrefilter);
        width /= 2;
        height /= 2;

        int fromId = bloomPrefilterId, toId = bloomPyramidId + 1;
        int i;
        for(i = 0; i < bloom.maxIterations; i++)
        {
            if(height < bloom.downscaleLimit || width < bloom.downscaleLimit)
            {
                break;
            }
            int midId = toId - 1;
            buffer.GetTemporaryRT(midId, width, height, 0, FilterMode.Bilinear, format);
            buffer.GetTemporaryRT(toId, width, height, 0, FilterMode.Bilinear, format);
            Draw(fromId, midId, Pass.BloomHorizontal);
            Draw(midId, toId, Pass.BloomVertical);
            fromId = toId;
            toId += 2;
            width /= 2;
            height /= 2;
        }
        buffer.ReleaseTemporaryRT(bloomPrefilterId);
        buffer.SetGlobalFloat(bloomBicubicUpsamplingId, bloom.bicubicUpsampling ? 1f : 0f);
        Pass combinePass, finalPass;
        float finalIntensity;
        if(bloom.mode == PostFXSettings.BloomSettings.Mode.Additive)
        {
            combinePass = finalPass = Pass.BloomAdd;
            buffer.SetGlobalFloat(bloomIntensityId, 1f);
            finalIntensity = bloom.intensity;
        }
        else
        {
            combinePass = Pass.BloomScatter;
            finalPass = Pass.BloomScatterFinal;
            buffer.SetGlobalFloat(bloomIntensityId, bloom.scatter);
            finalIntensity = Mathf.Min(bloom.intensity, 0.95f);
        }
        if(i > 1)
        {
            //Draw(fromId, BuiltinRenderTextureType.CameraTarget, Pass.Copy);
            buffer.ReleaseTemporaryRT(fromId - 1);
            toId -= 5;

            // releasing the texture use for pyramid level
            for (i -= 1; i > 0; i--)
            {
                buffer.SetGlobalTexture(fxSource2Id, toId + 1);
                Draw(fromId, toId, combinePass);
                buffer.ReleaseTemporaryRT(fromId);
                buffer.ReleaseTemporaryRT(toId + 1);
                fromId = toId;
                toId -= 2;
            }
        }
        else
        {
            buffer.ReleaseTemporaryRT(bloomPyramidId);
        }
        buffer.SetGlobalFloat(bloomIntensityId, finalIntensity);
        buffer.SetGlobalTexture(fxSource2Id, sourceId);
        buffer.GetTemporaryRT(bloomResultId, bufferSize.x, bufferSize.y, 0, FilterMode.Bilinear, format);
        Draw(fromId, bloomResultId, finalPass);
        buffer.ReleaseTemporaryRT(fromId);
        buffer.EndSample("Bloom");
        return true;
    }

    void ConfigureColorAdjustments()
    {
        ColorAdjustmentsSettings colorAdjustments = settings.ColorAdjustments;
        buffer.SetGlobalVector(colorAdjustmentsId, new Vector4(
                               Mathf.Pow(2f, colorAdjustments.postExposure),
                               colorAdjustments.contrast * 0.01f + 1f,
                               colorAdjustments.hueShift * (1f / 360f),
                               colorAdjustments.saturation * 0.01f + 1f));
        buffer.SetGlobalColor(colorFilterId, colorAdjustments.colorFilter.linear);
    }

    void ConfigureWhiteBalance()
    {
        WhiteBalanceSettings whiteBalance = settings.WhiteBalance;
        buffer.SetGlobalVector(whiteBalanceId, ColorUtils.ColorBalanceToLMSCoeffs(whiteBalance.temperature, whiteBalance.tint));
    }

    void ConfigureSplitToning()
    {
        SplitToningSettings splitToning = settings.SplitToning;
        Color splitColor = splitToning.shadows;
        splitColor.a = splitToning.balance * 0.01f;
        buffer.SetGlobalColor(splitToningShadowsId, splitColor);
        buffer.SetGlobalColor(splitToningHighlightsId, splitToning.highlights);
    }

    void ConfigureChannelMixer()
    {
        ChannelMixerSettings channelMixer = settings.ChannelMixer;
        buffer.SetGlobalVector(channelMixerRedId, channelMixer.red);
        buffer.SetGlobalVector(channelMixerGreenId, channelMixer.green);
        buffer.SetGlobalVector(channelMixerBlueId, channelMixer.blue);
    }

    void ConfigureShadowsMidtonesHighlights()
    {
        ShadowsMidtonesHighlightsSettings smh = settings.ShadowsMidtonesHighlights;
        buffer.SetGlobalColor(smhShadowsId, smh.shadows.linear);
        buffer.SetGlobalColor(smhMidtonesId, smh.midtones.linear);
        buffer.SetGlobalColor(smhHighlightsId, smh.highlights.linear);
        buffer.SetGlobalVector(smhRangeId, new Vector4(smh.shadowsStart, smh.shadowsEnd, smh.highlightsStart, smh.highlightsEnd));
    }

    void ConfigureScannerEffect()
    {
        ScannerEffectSettings se = settings.ScannerEffect;

        // calculation goes in here before sending to GPU
        
        se.scanDistance += Time.deltaTime * 50;
        //camera.depthTextureMode = DepthTextureMode.Depth; //copy the depth info from depth-buffer to RT
        
        float camFar = camera.farClipPlane;
        float camFov = camera.fieldOfView;
        float camAspect = camera.aspect;

        float fovWHalf = camFov * 0.5f;

        Vector3 toRight = camera.transform.right * Mathf.Tan(fovWHalf * Mathf.Deg2Rad) * camAspect;
        Vector3 toTop = camera.transform.up * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

        Vector3 topLeft = (camera.transform.forward - toRight + toTop);
        float camScale = topLeft.magnitude * camFar;

        topLeft.Normalize();
        topLeft *= camScale;

        Vector3 topRight = (camera.transform.forward + toRight + toTop);
        topRight.Normalize();
        topRight *= camScale;

        Vector3 bottomRight = (camera.transform.forward + toRight - toTop);
        bottomRight.Normalize();
        bottomRight *= camScale;

        Vector3 bottomLeft = (camera.transform.forward - toRight - toTop);
        bottomLeft.Normalize();
        bottomLeft *= camScale;

        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.MultiTexCoord(1, bottomLeft);
        GL.Vertex3(0.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.MultiTexCoord(1, bottomRight);
        GL.Vertex3(1.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.MultiTexCoord(1, topRight);
        GL.Vertex3(1.0f, 1.0f, 0.0f);

        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.MultiTexCoord(1, topLeft);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        buffer.SetGlobalFloat(seScanDistanceId, se.scanDistance);
        buffer.SetGlobalFloat(seScanWidthId, se.scanWidth);
        buffer.SetGlobalFloat(seLeadingEdgeSharpnessId, se.leadingEdgeSharpness);
        buffer.SetGlobalColor(seLeadingEdgeColorId, se.leadingEdgeColor);
        buffer.SetGlobalColor(seMidColorId, se.midColor);
        buffer.SetGlobalColor(seTrailColorId, se.trailColor);
        buffer.SetGlobalColor(seHorizontalBarColorId, se.horizontalBarColor);
        
    }

    void DoFinal(int sourceId)
    {
        ConfigureColorAdjustments();
        ConfigureWhiteBalance();
        ConfigureSplitToning();
        ConfigureChannelMixer();
        ConfigureShadowsMidtonesHighlights();

        int lutHeight = colorLUTResolution;
        int lutWidth = lutHeight * lutHeight;
        buffer.GetTemporaryRT(
            colorGradingLUTId, lutWidth, lutHeight, 0,
            FilterMode.Bilinear, RenderTextureFormat.DefaultHDR
        );
        buffer.SetGlobalVector(colorGradingLUTParametersId, new Vector4(
            lutHeight, 0.5f / lutWidth, 0.5f / lutHeight, lutHeight / (lutHeight - 1f)
        ));

        ToneMappingSettings.Mode mode = settings.ToneMapping.mode;
        Pass pass = Pass.ColorGradingNone + (int)mode;
        buffer.SetGlobalFloat(
            colorGradingLUTInLogId, useHDR && pass != Pass.ColorGradingNone ? 1f : 0f
        );
        Draw(sourceId, colorGradingLUTId, pass);

        buffer.SetGlobalVector(colorGradingLUTParametersId,
            new Vector4(1f / lutWidth, 1f / lutHeight, lutHeight - 1f)
        );
        buffer.SetGlobalFloat(finalSrcBlendId, 1f);
        buffer.SetGlobalFloat(finalDstBlendId, 0f);
        if (fxaa.enabled)
        {
            buffer.GetTemporaryRT(colorGradingResultId, bufferSize.x, bufferSize.y, 0,
                                  FilterMode.Bilinear, RenderTextureFormat.Default);
            Draw(sourceId, colorGradingResultId, Pass.ApplyColorGrading);
        }
        if(bufferSize.x == camera.pixelWidth)
        {
            if (fxaa.enabled)
            {
                DrawFinal(colorGradingResultId, Pass.FXAA);
                buffer.ReleaseTemporaryRT(colorGradingResultId);
            }
            else
            {
                DrawFinal(sourceId, Pass.ApplyColorGrading);
            }
        }
        else
        {
            buffer.GetTemporaryRT(finalResultId, bufferSize.x, bufferSize.y, 0, FilterMode.Bilinear, RenderTextureFormat.Default);
            if (fxaa.enabled)
            {
                Draw(colorGradingResultId, finalResultId, Pass.FXAA);
                buffer.ReleaseTemporaryRT(colorGradingResultId);
            }
            else
            {
                Draw(sourceId, finalResultId, Pass.ApplyColorGrading);
            }
            bool bicubicSampling =
                bicubicRescaling == CameraBufferSettings.BicubicRescalingMode.UpAndDown ||
                bicubicRescaling == CameraBufferSettings.BicubicRescalingMode.UpOnly &&
                bufferSize.x < camera.pixelWidth;
            buffer.SetGlobalFloat(copyBicubicId, bicubicSampling ? 1f : 0f);
            DrawFinal(finalResultId, Pass.FinalRescale);
            buffer.ReleaseTemporaryRT(finalResultId);
        }
        buffer.ReleaseTemporaryRT(colorGradingLUTId);
    }

    void DoScannerEffect(int sourceId)
    {
        ConfigureScannerEffect();
        buffer.SetGlobalTexture(fxSourceId, sourceId);
        buffer.Blit(sourceId, BuiltinRenderTextureType.CameraTarget, settings.Material, (int)Pass.ScannerEffect);
        //DoColorGradingAndToneMapping(sourceId);
    }
}
