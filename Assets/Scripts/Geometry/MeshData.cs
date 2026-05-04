using UnityEngine;

namespace Geometry
{
    // immutable mesh data container
    public class MeshData
    {
        public readonly Vector3[] Vertices;
        public readonly int[] Triangles;

        public MeshData(Vector3[] vertices, int[] triangles)
        {
            Vertices = vertices;
            Triangles = triangles;
        }
    }
}