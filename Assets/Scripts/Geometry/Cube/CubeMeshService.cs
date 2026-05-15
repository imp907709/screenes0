using Geometry.Cube;
using UnityEngine;

namespace Meshes
{
    // example mono usage
    public class CubeMeshService
    {
        public static Mesh Generate(float size = 1f)
        {
            // compose system
            var vertexGen = CubeVertexGenerator.Generate(size);
            var triangleGen = CubeTriangleGenerator.Generate();
            var mesh = ManualMesh.ManualMesh.Apply(vertexGen, triangleGen);
            
            return mesh;
        }
    }
}