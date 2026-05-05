using UnityEngine;

namespace Meshes.GeneralMesh
{
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

            return go;
        }
    }
}