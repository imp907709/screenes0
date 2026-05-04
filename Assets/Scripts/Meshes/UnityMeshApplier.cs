using Geometry;
using UnityEngine;

namespace Meshes
{
    // converts pure data into unity mesh
    public class UnityMeshApplier : IMeshApplier
    {
        public Mesh Apply(MeshData data)
        {
            var mesh = new Mesh
            {
                vertices = data.Vertices,
                triangles = data.Triangles
            };

            mesh.RecalculateNormals();
            return mesh;
        }
    }
}