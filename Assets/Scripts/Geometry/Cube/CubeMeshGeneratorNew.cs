using Geometry;
using UnityEngine;

namespace Geometry.Cube
{
    // composes vertex + triangle into mesh data
    public class CubeMeshDataGenerator : IMeshDataGenerator
    {
        private readonly IVertexGenerator _vertexGenerator;
        private readonly ITriangleGenerator _triangleGenerator;

        public CubeMeshDataGenerator(
            IVertexGenerator vertexGenerator,
            ITriangleGenerator triangleGenerator)
        {
            _vertexGenerator = vertexGenerator;
            _triangleGenerator = triangleGenerator;
        }

        public MeshData Generate(float size)
        {
            var vertices = _vertexGenerator.Generate(size);
            var triangles = _triangleGenerator.Generate();

            return new MeshData(vertices, triangles);
        }
        // Unity calls this when editor selection changes
        public void OnSelectionChange()
        {
        }
    }
}