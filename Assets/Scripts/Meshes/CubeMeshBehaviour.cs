using Geometry.Cube;
using UnityEngine;

namespace Meshes
{
    // example mono usage
    public class CubeMeshBehaviour
    {
        public Mesh Generate(float size = 1f)
        {
            // compose system
            var vertexGen = new CubeVertexGenerator();
            var triangleGen = new CubeTriangleGenerator();
            var dataGen = new CubeMeshDataGenerator(vertexGen, triangleGen);
            var applier = new UnityMeshApplier();

            // generate data
            var data = dataGen.Generate(size);

            // apply to mesh
            var mesh = applier.Apply(data);

            return mesh;
        }
    }
}