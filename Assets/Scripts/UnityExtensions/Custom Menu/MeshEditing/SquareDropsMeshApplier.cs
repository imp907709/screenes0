using Meshes.SquareDrops;
using Meshes.Voronoi;
using UnityEngine;

namespace UnityExtensions.Custom_Menu.MeshEditing
{
    public class SquareDropsMeshApplier
    {
        public static Mesh GenerateAndApply(
            Mesh mesh,
            float size,
            int count)
        {
            // 1. generate sites
            var siteGen = new SquareDropsGenerator();
            var sites = siteGen.GenerateSites(count, size);

            // 2. build geometry
            var builder = new SquareDropsGenerator();
            var data = builder.Build(sites);

            // 3. apply to mesh
            Apply(mesh, data);
            return mesh;
        }
        
        public static Mesh Apply(Mesh mesh, SquareDropsMeshData data)
        {
            mesh.vertices = data.Vertices;
            mesh.triangles = data.Triangles;

            mesh.RecalculateNormals();

            return mesh;
        }
    }
}