using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;
public class FinalPass
{
    CameraRenderer renderer;

    CameraSettings.FinalBlendMode finalBlendMode;
    void Render(RenderGraphContext context)
    {
        renderer.DrawFinal(finalBlendMode);
        renderer.ExecuteBuffer();
    }

    public static void Record(RenderGraph renderGraph, 
        CameraRenderer renderer, CameraSettings.FinalBlendMode finalBlendMode)
    {
        using RenderGraphBuilder builder = renderGraph.AddRenderPass("Final", out FinalPass pass);
        pass.renderer = renderer;
        pass.finalBlendMode = finalBlendMode;
        builder.SetRenderFunc<FinalPass>((pass, context) => pass.Render(context));
    }
}
