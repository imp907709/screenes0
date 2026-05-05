using System.Collections.Generic;
using UnityEngine;

public static class OctagonMeshBuilder
{
    public static Mesh Build(World world)
    {
        Dictionary<Vector3, int> vertexMap = new Dictionary<Vector3, int>();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        foreach (var cell in world.Cells)
        {
            AddOctagon(cell, world, vertexMap, vertices, triangles);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    /// <summary>
    /// Same octagon layout per cell, but vertices are not merged across cells so each patch can carry its biome color.
    /// </summary>
    public static Mesh BuildWithBiomeColors(World world)
    {
        var vertices = new List<Vector3>();
        var colors = new List<Color>();
        var triangles = new List<int>();

        foreach (var cell in world.Cells)
            AddOctagonColored(cell, vertices, colors, triangles);

        var mesh = new Mesh { name = "BiomeOctagonsColored" };
        if (vertices.Count == 0)
            return mesh;

        mesh.SetVertices(vertices);
        mesh.SetColors(colors);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    private static void AddOctagon(
        Cell cell,
        World world,
        Dictionary<Vector3, int> map,
        List<Vector3> verts,
        List<int> tris)
    {
        float size = 1f;

        Vector3 center = cell.Position + Vector3.up * cell.Height;

        int centerIndex = AddVertex(center, map, verts);

        List<int> ring = new List<int>();

        for (int i = 0; i < 8; i++)
        {
            float angle = i * Mathf.PI * 2f / 8f;

            Vector3 v = new Vector3(
                Mathf.Cos(angle) * size,
                0,
                Mathf.Sin(angle) * size
            );

            v += cell.Position;
            v.y = cell.Height;

            ring.Add(AddVertex(v, map, verts));
        }

        for (int i = 0; i < 8; i++)
        {
            int next = (i + 1) % 8;

            tris.Add(centerIndex);
            tris.Add(ring[i]);
            tris.Add(ring[next]);
        }
    }

    private static int AddVertex(
        Vector3 v,
        Dictionary<Vector3, int> map,
        List<Vector3> verts)
    {
        if (map.TryGetValue(v, out int index))
            return index;

        index = verts.Count;
        verts.Add(v);
        map[v] = index;
        return index;
    }

    private static void AddOctagonColored(
        Cell cell,
        List<Vector3> verts,
        List<Color> cols,
        List<int> tris)
    {
        Color tint = CellBiomeColor(cell);
        float size = 1f;

        Vector3 center = cell.Position + Vector3.up * cell.Height;
        int centerIndex = verts.Count;
        verts.Add(center);
        cols.Add(tint);

        var ring = new int[8];
        for (int i = 0; i < 8; i++)
        {
            float angle = i * Mathf.PI * 2f / 8f;
            Vector3 v = new Vector3(
                Mathf.Cos(angle) * size,
                0f,
                Mathf.Sin(angle) * size);
            v += cell.Position;
            v.y = cell.Height;

            ring[i] = verts.Count;
            verts.Add(v);
            cols.Add(tint);
        }

        for (int i = 0; i < 8; i++)
        {
            int next = (i + 1) % 8;
            tris.Add(centerIndex);
            tris.Add(ring[i]);
            tris.Add(ring[next]);
        }
    }

    private static Color CellBiomeColor(Cell cell)
    {
        if (cell?.Biome != null)
            return cell.Biome.Color;

        return new Color(0.25f, 0.25f, 0.25f, 1f);
    }
}