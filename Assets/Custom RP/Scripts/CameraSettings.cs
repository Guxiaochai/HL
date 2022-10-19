using System;
using UnityEngine.Rendering;

[Serializable]
public class CameraSettings
{
    public bool overridePostFX = false;

    public PostFXSettings postFXSettings = default;

    [Serializable]
    public struct FinalBlendMode
    {
        public BlendMode source, destination;
    }

    public FinalBlendMode finalBlendMode = new FinalBlendMode
    {
        source = BlendMode.One,
        destination = BlendMode.Zero
    };
}
