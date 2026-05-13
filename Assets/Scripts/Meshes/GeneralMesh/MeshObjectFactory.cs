using UnityEngine;

namespace Meshes.GeneralMesh
{
    // build arbitrary mesh from mesh and material
    public static class MeshObjectFactory
    {
        public static GameObject Create(
            Mesh mesh,
            Material material,
            string name = "ProceduralMesh")
        {
            var go = new GameObject(name);

            var filter = go.AddComponent<MeshFilter>();
            var renderer = go.AddComponent<MeshRenderer>();

            filter.mesh = mesh;
            renderer.material = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            renderer.receiveShadows = true;

            return go;
        }
    }
}