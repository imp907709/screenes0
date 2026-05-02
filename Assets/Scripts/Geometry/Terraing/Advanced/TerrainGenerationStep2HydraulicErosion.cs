using UnityEngine;

public readonly struct TerrainHydraulicErosionStepSettings
{
    public readonly int dropletCount;
    public readonly int maxStepsPerDroplet;
    public readonly float carryStrength;
    public readonly uint seed;

    public TerrainHydraulicErosionStepSettings(int dropletCount, int maxStepsPerDroplet, float carryStrength, uint seed)
    {
        this.dropletCount = dropletCount;
        this.maxStepsPerDroplet = maxStepsPerDroplet;
        this.carryStrength = carryStrength;
        this.seed = seed;
    }
}

/// <summary>Step 2: discrete hydraulic erosion on mesh heights (regular grid).</summary>
public sealed class TerrainGenerationStep2HydraulicErosion
{
    public void Apply(Mesh mesh, int vertexGridX, int vertexGridZ, in TerrainHydraulicErosionStepSettings s)
    {
        if (mesh == null || !TerrainHeightmapGrid.VertexCountMatches(mesh.vertexCount, vertexGridX, vertexGridZ))
            return;

        int width = vertexGridX;
        int depth = vertexGridZ;
        if (width < 3 || depth < 3 || s.dropletCount <= 0)
            return;

        var verts = mesh.vertices;
        float[,] heights = TerrainHeightmapGrid.FromVertices(verts, width, depth);

        RunHydraulic(heights, width, depth, s.dropletCount, Mathf.Max(1, s.maxStepsPerDroplet), s.carryStrength, s.seed);

        TerrainHeightmapGrid.WriteYToVertices(verts, heights, width, depth);
        mesh.vertices = verts;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    private static void RunHydraulic(
        float[,] heights,
        int width,
        int depth,
        int dropletCount,
        int maxStepsPerDroplet,
        float carryStrength,
        uint seed)
    {
        var rng = new System.Random((int)(seed ^ (seed >> 16)));

        for (int d = 0; d < dropletCount; d++)
        {
            int cx = 1 + rng.Next(width - 2);
            int cz = 1 + rng.Next(depth - 2);

            for (int step = 0; step < maxStepsPerDroplet; step++)
            {
                float hHere = heights[cx, cz];
                int bx = cx;
                int bz = cz;
                float best = hHere;

                for (int dz = -1; dz <= 1; dz++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (dx == 0 && dz == 0)
                            continue;
                        int nx = cx + dx;
                        int nz = cz + dz;
                        if ((uint)nx >= (uint)width || (uint)nz >= (uint)depth)
                            continue;
                        float hn = heights[nx, nz];
                        if (hn < best)
                        {
                            best = hn;
                            bx = nx;
                            bz = nz;
                        }
                    }
                }

                if (best >= hHere - 1e-9f)
                    break;

                float dh = hHere - best;
                if (dh <= 1e-12f)
                    break;

                // Move a fraction of the downhill step; do not cap with hHere*0.25 — when hHere < 0 that cap goes negative and flips the transfer.
                float amt = dh * Mathf.Min(carryStrength, 0.5f);
                if (amt < 1e-12f)
                    break;

                heights[cx, cz] -= amt;
                heights[bx, bz] += amt;
                cx = bx;
                cz = bz;
            }
        }
    }
}
