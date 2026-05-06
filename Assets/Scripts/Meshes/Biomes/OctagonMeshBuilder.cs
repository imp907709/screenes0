using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
        if (vertices.Count > ushort.MaxValue)
            mesh.indexFormat = IndexFormat.UInt32;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    /// <summary>
    /// Per-cell octagons with biome vertex colors, then weld same (position, color) to drop redundant verts.
    /// </summary>
    public static Mesh BuildWithBiomeColors(World world)
    {
        var vertices = new List<Vector3>();
        var colors = new List<Color>();
        var triangles = new List<int>();

        foreach (var cell in world.Cells)
            AddHexagonColored(cell, vertices, colors, triangles);

        WeldVerticesByPositionAndColor(vertices, colors, triangles);

        var mesh = new Mesh { name = "BiomeOctagonsColored" };
        if (vertices.Count == 0)
            return mesh;

        if (vertices.Count > ushort.MaxValue)
            mesh.indexFormat = IndexFormat.UInt32;

        mesh.SetVertices(vertices);
        mesh.SetColors(colors);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    /// <summary>
    /// Merges vertices that coincide in space and share the same color (same biome along shared edges / overlaps).
    /// Drops degenerate triangles. Keeps different colors at the same position separate so biome borders stay sharp.
    /// </summary>
    private static void WeldVerticesByPositionAndColor(List<Vector3> vertices, List<Color> colors, List<int> triangles)
    {
        int oldCount = vertices.Count;
        if (oldCount == 0 || colors.Count != oldCount)
            return;

        var keyToNew = new Dictionary<WeldKey, int>(oldCount);
        var remap = new int[oldCount];
        var newVerts = new List<Vector3>(oldCount / 4);
        var newCols = new List<Color>(oldCount / 4);

        for (int i = 0; i < oldCount; i++)
        {
            var key = new WeldKey(vertices[i], colors[i]);
            if (!keyToNew.TryGetValue(key, out int ni))
            {
                ni = newVerts.Count;
                keyToNew[key] = ni;
                newVerts.Add(vertices[i]);
                newCols.Add(colors[i]);
            }

            remap[i] = ni;
        }

        var newTris = new List<int>(triangles.Count);
        for (int t = 0; t < triangles.Count; t += 3)
        {
            int a = remap[triangles[t]];
            int b = remap[triangles[t + 1]];
            int c = remap[triangles[t + 2]];
            if (a == b || b == c || a == c)
                continue;
            newTris.Add(a);
            newTris.Add(b);
            newTris.Add(c);
        }

        vertices.Clear();
        vertices.AddRange(newVerts);
        colors.Clear();
        colors.AddRange(newCols);
        triangles.Clear();
        triangles.AddRange(newTris);
    }

    private readonly struct WeldKey : IEquatable<WeldKey>
    {
        public readonly int px, py, pz;
        public readonly int cr, cg, cb, ca;

        public WeldKey(Vector3 p, Color c)
        {
            px = Mathf.RoundToInt(p.x * 4096f);
            py = Mathf.RoundToInt(p.y * 4096f);
            pz = Mathf.RoundToInt(p.z * 4096f);
            cr = Mathf.RoundToInt(c.r * 255f);
            cg = Mathf.RoundToInt(c.g * 255f);
            cb = Mathf.RoundToInt(c.b * 255f);
            ca = Mathf.RoundToInt(c.a * 255f);
        }

        public bool Equals(WeldKey o) =>
            px == o.px && py == o.py && pz == o.pz && cr == o.cr && cg == o.cg && cb == o.cb && ca == o.ca;

        public override bool Equals(object obj) => obj is WeldKey w && Equals(w);

        public override int GetHashCode()
        {
            unchecked
            {
                int h = px;
                h = (h * 397) ^ py;
                h = (h * 397) ^ pz;
                h = (h * 397) ^ cr;
                h = (h * 397) ^ cg;
                h = (h * 397) ^ cb;
                h = (h * 397) ^ ca;
                return h;
            }
        }
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
    
    private static void AddHexagonColored(
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

        const int sides = 6;
        var ring = new int[sides];

        for (int i = 0; i < sides; i++)
        {
            float angle = i * Mathf.PI * 2f / sides;

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

        for (int i = 0; i < sides; i++)
        {
            int next = (i + 1) % sides;

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