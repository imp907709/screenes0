using Meshes.SquareDrops;
using UnityEngine;

namespace UnityExtensions.Custom_Menu.CustomUI.MeshEditing
{
    public class SquareDropsMeshApplier
    {
        /// <summary>
        /// Replaces mesh geometry with random axis-aligned squares on the mesh XZ footprint (same idea as Voronoi menu: pass a writable mesh clone).
        /// </summary>
        public static Mesh GenerateAndApply(Mesh mesh, float size, int count, int seed)
        {
            Bounds b = mesh.bounds;
            Vector2 min = new Vector2(b.min.x, b.min.z);
            Vector2 max = new Vector2(b.max.x, b.max.z);
            float spanX = max.x - min.x;
            float spanZ = max.y - min.y;
            if (spanX < 1e-4f || spanZ < 1e-4f)
            {
                min = Vector2.zero;
                max = new Vector2(size, size);
                spanX = spanZ = size;
            }

            float yPlane = b.center.y;
            // ~0.5 world units on a ~10-unit-wide mesh (old behaviour); scales with footprint.
            float halfExtent = Mathf.Clamp(0.5f * Mathf.Min(spanX, spanZ) / 10f, 0.15f, 4f);

            var gen = new SquareDropsGenerator();
            var sites = gen.GenerateSites(count, min, max, seed);
            var data = gen.Build(sites, yPlane, halfExtent);

            Apply(mesh, data);
            return mesh;
        }

        public static Mesh Apply(Mesh mesh, SquareDropsMeshData data)
        {
            mesh.Clear();
            mesh.SetVertices(data.Vertices);
            mesh.SetTriangles(data.Triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}