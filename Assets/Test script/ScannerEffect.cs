using UnityEngine;
using UnityEngine.Rendering;

public class ScannerEffect
{
    const string bufferName = "Scanner Effect";

    CommandBuffer buffer = new CommandBuffer { name = bufferName };

    ScriptableRenderContext context;

    Camera camera;

    ScannerEffectSettings settings;

    //public bool IsActive => settings != null;
    public bool IsActive => false;

    public void Setup(ScriptableRenderContext context, Camera camera, ScannerEffectSettings settings)
    {
        this.context = context;
        this.camera = camera;
        this.settings = settings;
    }

    public void Render(int sourceId)
    {
        buffer.Blit(sourceId, BuiltinRenderTextureType.CameraTarget);
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
}
