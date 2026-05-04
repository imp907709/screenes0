using System.Collections.Generic;
using UnityEngine;
using Meshes.Voronoi;
using Meshes.Voronoi.VoronatorSharp;

public static class VoronoiApplier
{
    public static void Generate(
        int count,
        float size,
        int seed,
        Mesh mesh)
    {
        if (mesh == null)
        {
            Debug.LogError("Mesh is null");
            return;
        }

        // 1. generate sites
        var sites = GenerateSites(count, size, seed);
        
        // 2. build voronoi
        var voronator = new Voronator(
            sites,
            new Vector2(0, 0),
            new Vector2(size, size)
        );
        
        // 3. build mesh data
        var vertices = new List<Vector3>();
        var triangles = new List<int>();

        for (int i = 0; i < sites.Count; i++)
        {
            var poly = voronator.GetClippedPolygon(i);

            if (poly == null || poly.Count < 3)
                continue;

            int start = vertices.Count;

            for (int p = 0; p < poly.Count; p++)
                vertices.Add(new Vector3(poly[p].x, 0f, poly[p].y));

            for (int p = 1; p < poly.Count - 1; p++)
            {
                triangles.Add(start);
                triangles.Add(start + p);
                triangles.Add(start + p + 1);
            }
        }

        // 4. apply to EXISTING mesh
        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    // site generator
    private static List<Vector2> GenerateSites(int count, float size, int seed)
    {
        var sites = new List<Vector2>();

        var old = Random.state;
        Random.InitState(seed);

        for (int i = 0; i < count; i++)
        {
            sites.Add(new Vector2(
                Random.Range(0f, size),
                Random.Range(0f, size)
            ));
        }

        Random.state = old;

        return sites;
    }
}