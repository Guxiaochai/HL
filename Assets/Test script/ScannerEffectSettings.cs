using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Rendering/Scanner Effect Settings")]
public class ScannerEffectSettings : ScriptableObject
{
    [SerializeField]
    Shader shader = default;

    [System.NonSerialized]
    Material material;

    [Serializable]
    public struct NormalSettings
    {
        public float scanDistance;

        public float scanWidth;

        public float leadingEdgeSharpness;

        public Color leadingEdgeColor;

        public Color midColor;

        public Color trailColor;

        public Color horizontalBarColor;
    }

    [SerializeField]
    NormalSettings normal = new NormalSettings { };

    public NormalSettings Normal => normal;

    public Material Material
    {
        get
        {
            if (material == null && shader != null)
            {
                material = new Material(shader);
                material.hideFlags = HideFlags.HideAndDontSave;
            }
            return material;
        }
    }
}
