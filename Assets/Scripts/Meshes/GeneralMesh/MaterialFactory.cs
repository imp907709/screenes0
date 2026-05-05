using UnityEngine;

namespace Meshes.GeneralMesh
{
    public static class MaterialFactory
    {
        private static Material _defaultMaterial;

        public static Material GetDefaultMaterial()
        {
            if (_defaultMaterial != null)
                return _defaultMaterial;

            // Fallback to built-in Standard shader
            Shader shader =
                Shader.Find("Unlit/Color") ??
                Shader.Find("Sprites/Default") ??
                Shader.Find("Standard") ??
                Shader.Find("HDRP/Lit") ??
                Shader.Find("Universal Render Pipeline/Unlit");
            
            _defaultMaterial = new Material(shader);

            Debug.Log("MaterialFactory Fallback material");
            return _defaultMaterial;
        }
        
        public static void ApplyRandomColor(Material mat)
        {
            var color = UnityEngine.Random.ColorHSV();

            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", color);
            else if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", color);
        }
        
        public static Material GetDefault(Color? color = null)
        {
            if (_defaultMaterial == null)
            {
                _defaultMaterial = CreateBaseMaterial();
            }

            if (color == null)
                return _defaultMaterial;

            // create instance only if color differs
            var matInstance = new Material(_defaultMaterial);
            ApplyColor(matInstance, color.Value);
            return matInstance;
        }

        private static Material CreateBaseMaterial()
        {
            var baseMat =
                UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline?.defaultMaterial;

            if (baseMat != null)
                return new Material(baseMat);

            // fallback (built-in pipeline)
            var shader = Shader.Find("Standard");
            return new Material(shader);
        }

        private static void ApplyColor(Material mat, Color color)
        {
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", color);
            else if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", color);
        }
    }
}