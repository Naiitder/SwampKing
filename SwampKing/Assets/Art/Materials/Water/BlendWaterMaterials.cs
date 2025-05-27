using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class BlendWaterMaterials : MonoBehaviour
{
    [System.Serializable]
    public class AlphaMaterialRange
    {
        public Material material;
        [Range(0f, 1f)] public float minAlpha;
        [Range(0f, 1f)] public float maxAlpha;
    }

    public Texture2D alphaTexture;
    public AlphaMaterialRange[] materialRanges;

    private Material blendMaterial;

    void Start()
    {
        ApplyBlend();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        ApplyBlend();
    }
#endif

    void ApplyBlend()
    {
        if (alphaTexture == null || materialRanges.Length == 0) return;

        Shader shader = Shader.Find("Custom/AlphaBlend3Materials");
        if (!shader) {
            Debug.LogError("Custom/AlphaBlendShader not found.");
            return;
        }

        if (blendMaterial == null)
            blendMaterial = new Material(shader);

        blendMaterial.SetTexture("_AlphaTex", alphaTexture);

        for (int i = 0; i < materialRanges.Length; i++)
        {
            blendMaterial.SetTexture($"_Tex{i}", materialRanges[i].material.mainTexture);
            blendMaterial.SetFloat($"_Min{i}", materialRanges[i].minAlpha);
            blendMaterial.SetFloat($"_Max{i}", materialRanges[i].maxAlpha);
        }

        Renderer renderer = GetComponent<Renderer>();
        renderer.sharedMaterial = blendMaterial;
    }
}