using UnityEngine;
using UnityEngine.Rendering;

public partial class PostFXStack
{
    enum Pass
    {
        BloomHorizontal,
        Copy,
        BloomVertical
    }

    const int maxBloomPyramidLevels = 16;
    int bloomPyramidId;

    int fxSourceId = Shader.PropertyToID("_PostFXSource");

    const string bufferName = "Emmer Post FX";

    CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };

    ScriptableRenderContext context;

    Camera camera;

    PostFXSettings settings;

    public bool IsActive => settings != null;

    public PostFXStack()
    {
        bloomPyramidId = Shader.PropertyToID("_BloomPyramid0");
        for(int i = 0; i < maxBloomPyramidLevels * 2; i++)
        {
            Shader.PropertyToID("_BloomPyramid" + i);
        }
    }

    public void Setup(ScriptableRenderContext context, Camera camera, PostFXSettings settings)
    {
        this.context = context;
        this.camera = camera;
        this.settings = camera.cameraType <= CameraType.SceneView ? settings : null;
        ApplySceneViewState();
    }

    public void Render(int sourceId)
    {
        //buffer.Blit(sourceId, BuiltinRenderTextureType.CameraTarget);
        //Draw(sourceId, BuiltinRenderTextureType.CameraTarget, Pass.Copy);
        DoBloom(sourceId);

        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    void Draw(RenderTargetIdentifier from, RenderTargetIdentifier to, Pass pass)
    {
        buffer.SetGlobalTexture(fxSourceId, from);
        buffer.SetRenderTarget(to, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
        buffer.DrawProcedural(Matrix4x4.identity, settings.Material, (int)pass, MeshTopology.Triangles, 3);
    }

    void DoBloom(int sourceId)
    {
        buffer.BeginSample("Bloom");
        PostFXSettings.BloomSettings bloom = settings.Bloom;
        int width = camera.pixelWidth / 2, height = camera.pixelHeight / 2;
        RenderTextureFormat format = RenderTextureFormat.Default;
        int fromId = sourceId, toId = bloomPyramidId + 1;

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
        Draw(fromId, BuiltinRenderTextureType.CameraTarget, Pass.Copy);

        // releasing the texture use for pyramid level
        for(i -= 1; i>=0; i--)
        {
            buffer.ReleaseTemporaryRT(fromId);
            buffer.ReleaseTemporaryRT(fromId - 1);
            fromId -= 2;
        }
        buffer.EndSample("Bloom");
    }
}
