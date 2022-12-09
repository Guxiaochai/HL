using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Profiling;
using UnityEditor;

partial class CameraRenderer
{
    partial void DrawGizmosBeforeFX();
    partial void DrawGizmosAfterFX();
    partial void DrawUnsupportedShaders();
    partial void PrepareForSceneWindow();
    partial void PrepareBuffer();
    partial void DisposeForEditor();

#if UNITY_EDITOR

    string SampleName { get; set; }

    static ShaderTagId[] legacyShaderTagId =
    {
        new ShaderTagId("Always"),
        new ShaderTagId("ForwardBase"),
        new ShaderTagId("PrepassBase"),
        new ShaderTagId("Vertex"),
        new ShaderTagId("VertexLMRGBM"),
        new ShaderTagId("VertexLM")
    };

    static Material errorMaterial;

    partial void DrawUnsupportedShaders()
    {
        if(errorMaterial == null)
        {
            errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
        }

        var drawingSettings = new DrawingSettings(legacyShaderTagId[0], new SortingSettings(camera))
        {
            overrideMaterial = errorMaterial
        };
        for(int i = 1; i < legacyShaderTagId.Length; i++)
        {
            drawingSettings.SetShaderPassName(i, legacyShaderTagId[i]);
        }
        var filteringSettings = FilteringSettings.defaultValue;

        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
    }

    partial void DrawGizmosBeforeFX()
    {
        if (Handles.ShouldRenderGizmos())
        {
            if (useIntermediateBuffer)
            {
                Draw(depthAttachmentId, BuiltinRenderTextureType.CameraTarget, true);
                ExecuteBuffer();
            }
            context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
        }
    }

    partial void DrawGizmosAfterFX()
    {
        if (Handles.ShouldRenderGizmos())
        {
            context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
        }
    }

    partial void PrepareForSceneWindow()
    {
        if(camera.cameraType == CameraType.SceneView)
        {
            ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
        }
    }

    partial void PrepareBuffer()
    {
        Profiler.BeginSample("Editor Only");
        buffer.name = camera.name;
        Profiler.EndSample();
    }

    partial void DisposeForEditor()
    {
        UnityEngine.Experimental.GlobalIllumination.Lightmapping.ResetDelegate();
    }

#else

    const string SampleName = bufferName;

#endif
}
