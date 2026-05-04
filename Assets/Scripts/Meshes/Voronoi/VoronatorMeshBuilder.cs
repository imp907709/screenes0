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

        if (count < 3)
        {
            Debug.LogWarning("VoronoiApplier: use at least 3 sites (Delaunay needs a non-degenerate point set).");
            return;
        }

        if (!TryBuildVoronoiContext(mesh, count, size, seed, out var sites, out var voronator, out float yFlat, out Bounds b, out bool usedBoundsFallback, out Vector2 clipMin, out Vector2 clipMax))
            return;

        LogSitesSummary(sites, clipMin, clipMax, yFlat, usedBoundsFallback, b);

        // build mesh data
        var vertices = new List<Vector3>();
        var triangles = new List<int>();

        int skippedNull = 0;
        int skippedSmall = 0;
        int cellsUsed = 0;
        bool loggedFirstPoly = false;

        for (int i = 0; i < sites.Count; i++)
        {
            var poly = voronator.GetClippedPolygon(i);

            if (poly == null)
            {
                skippedNull++;
                if (skippedNull <= 3)
                    Debug.LogWarning($"[VoronoiApplier] cell {i}: GetClippedPolygon returned null (site={sites[i]})");
                continue;
            }

            if (poly.Count < 3)
            {
                skippedSmall++;
                if (skippedSmall <= 3)
                    Debug.LogWarning($"[VoronoiApplier] cell {i}: clipped polygon vertex count {poly.Count} (< 3)");
                continue;
            }

            if (!loggedFirstPoly)
            {
                loggedFirstPoly = true;
                var corners = new System.Text.StringBuilder();
                int nShow = Mathf.Min(poly.Count, 6);
                for (int p = 0; p < nShow; p++)
                    corners.Append($" ({poly[p].x:F4},{poly[p].y:F4})");
                if (poly.Count > nShow)
                    corners.Append($" … +{poly.Count - nShow} verts");
                Debug.Log($"[VoronoiApplier] first clipped cell (index {i}) has {poly.Count} corners (XZ plane coords):{corners}");
            }

            cellsUsed++;

            int start = vertices.Count;

            for (int p = 0; p < poly.Count; p++)
                vertices.Add(new Vector3(poly[p].x, yFlat, poly[p].y));

            for (int p = 1; p < poly.Count - 1; p++)
            {
                triangles.Add(start);
                triangles.Add(start + p);
                triangles.Add(start + p + 1);
            }
        }

        Debug.Log(
            $"[VoronoiApplier] mesh build: cellsUsed={cellsUsed}/{sites.Count}, " +
            $"skippedNull={skippedNull}, skippedSmall={skippedSmall}, " +
            $"vertices={vertices.Count}, triangleIndices={triangles.Count} ({triangles.Count / 3} tris)");

        // 4. apply to EXISTING mesh
        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        if (vertices.Count == 0)
            Debug.LogError("VoronoiApplier: no clipped cells produced (sites may be degenerate or Voronator failed).");
        else if (vertices.Count >= 3)
        {
            var v0 = vertices[0];
            var v1 = vertices[1];
            var v2 = vertices[2];
            Debug.Log($"[VoronoiApplier] sample output verts (local space): {v0}, {v1}, {v2}");
        }
    }

    /// <summary>
    /// Clip each triangle in XZ to Voronator cell polygons; Y from barycentric interpolation on the source triangle.
    /// </summary>
    public static void GenerateCut(int count, float size, int seed, Mesh mesh)
    {
        if (mesh == null)
        {
            Debug.LogError("Mesh is null");
            return;
        }

        if (count < 3)
        {
            Debug.LogWarning("VoronoiApplier: use at least 3 sites (Delaunay needs a non-degenerate point set).");
            return;
        }

        if (!TryBuildVoronoiContext(mesh, count, size, seed, out var sites, out var voronator, out _, out Bounds b, out bool usedBoundsFallback, out Vector2 clipMin, out Vector2 clipMax))
            return;

        LogSitesSummary(sites, clipMin, clipMax, b.center.y, usedBoundsFallback, b);

        if (!VoronoiPlanarMeshCut.IsMeshEligibleForXZSlabCut(b))
        {
            Debug.LogWarning(
                "[VoronoiApplier] GenerateCut: mesh is not a thin horizontal slab (XZ footprint ≫ thickness in Y). " +
                "Cutting in XZ collapses vertical faces (e.g. on a cube), so the mesh was left unchanged. " +
                "Use a plane / terrain, or turn off \"Cut mesh\" and use flat Voronoi.");
            return;
        }

        int vertsBefore = mesh.vertexCount;
        int trisBefore = mesh.triangles.Length / 3;

        VoronoiPlanarMeshCut.CutMeshWithVoronoi(mesh, voronator, sites.Count);

        Debug.Log(
            $"[VoronoiApplier] cut: input ~{vertsBefore} verts / {trisBefore} tris -> output {mesh.vertexCount} verts / {mesh.triangles.Length / 3} tris");

        if (mesh.vertexCount == 0)
            Debug.LogError("VoronoiApplier.GenerateCut: produced empty mesh.");
    }

    private static bool TryBuildVoronoiContext(
        Mesh mesh,
        int count,
        float size,
        int seed,
        out List<Vector2> sites,
        out Voronator voronator,
        out float yFlat,
        out Bounds bounds,
        out bool usedBoundsFallback,
        out Vector2 clipMin,
        out Vector2 clipMax)
    {
        sites = null;
        voronator = null;
        yFlat = 0f;
        bounds = default;
        usedBoundsFallback = false;
        clipMin = default;
        clipMax = default;

        if (mesh == null || count < 3)
            return false;

        bounds = mesh.bounds;
        clipMin = new Vector2(bounds.min.x, bounds.min.z);
        clipMax = new Vector2(bounds.max.x, bounds.max.z);
        float spanX = clipMax.x - clipMin.x;
        float spanZ = clipMax.y - clipMin.y;
        usedBoundsFallback = spanX < 1e-4f || spanZ < 1e-4f;
        if (usedBoundsFallback)
        {
            clipMin = Vector2.zero;
            clipMax = new Vector2(size, size);
        }

        yFlat = bounds.center.y;
        sites = GenerateSites(count, clipMin, clipMax, seed);
        voronator = new Voronator(sites, clipMin, clipMax);
        return true;
    }

    private static void LogSitesSummary(
        List<Vector2> sites,
        Vector2 clipMin,
        Vector2 clipMax,
        float yFlat,
        bool usedBoundsFallback,
        Bounds meshBounds)
    {
        Vector2 sMin = sites[0];
        Vector2 sMax = sites[0];
        for (int i = 1; i < sites.Count; i++)
        {
            sMin = Vector2.Min(sMin, sites[i]);
            sMax = Vector2.Max(sMax, sites[i]);
        }

        int show = Mathf.Min(3, sites.Count);
        var first = new System.Text.StringBuilder();
        for (int i = 0; i < show; i++)
            first.Append($" ({sites[i].x:F4},{sites[i].y:F4})");

        Debug.Log(
            $"[VoronoiApplier] input mesh.bounds min={meshBounds.min} max={meshBounds.max} center={meshBounds.center}; " +
            $"clip XZ min=({clipMin.x:F4},{clipMin.y:F4}) max=({clipMax.x:F4},{clipMax.y:F4}) " +
            $"(fallbackZeroSpan={usedBoundsFallback}); yFlat={yFlat:F4}; " +
            $"sites count={sites.Count} first{show}={first}; sites XZ span min=({sMin.x:F4},{sMin.y:F4}) max=({sMax.x:F4},{sMax.y:F4})");
    }

    // site generator (uniform in clip rectangle, inclusive-ish of interior)
    private static List<Vector2> GenerateSites(int count, Vector2 min, Vector2 max, int seed)
    {
        var sites = new List<Vector2>();

        var old = Random.state;
        Random.InitState(seed);

        for (int i = 0; i < count; i++)
        {
            sites.Add(new Vector2(
                Random.Range(min.x, max.x),
                Random.Range(min.y, max.y)
            ));
        }

        Random.state = old;

        return sites;
    }
}