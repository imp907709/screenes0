using Meshes.Voronoi;
using UnityEngine;

namespace UnityExtensions.Custom_Menu.MeshEditing
{
    public class VoronoiMeshApplier
    {
        public static Mesh GenerateAndApply(
            Mesh mesh,
            float size,
            int count)
        {
            // 1. generate sites
            var siteGen = new VoronoiDataGenerator();
            var sites = siteGen.GenerateSites(count, size);

            // 2. build geometry
            var builder = new VoronoiGeometryBuilder();
            var data = builder.Build(sites);

            // 3. apply to mesh
            Apply(mesh, data);
            return mesh;
        }
        
        public static Mesh Apply(Mesh mesh, VoronoiMeshData data)
        {
            mesh.vertices = data.Vertices;
            mesh.triangles = data.Triangles;

            mesh.RecalculateNormals();

            return mesh;
        }
    }
}