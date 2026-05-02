using UnityEngine;

public readonly struct TerrainThermalErosionStepSettings
{
    public readonly int iterations;
    public readonly float talusHeightDelta;
    public readonly float strength;

    public TerrainThermalErosionStepSettings(int iterations, float talusHeightDelta, float strength)
    {
        this.iterations = iterations;
        this.talusHeightDelta = talusHeightDelta;
        this.strength = strength;
    }
}

/// <summary>Step 3: thermal / talus erosion on mesh heights (regular grid).</summary>
public sealed class TerrainGenerationStep3ThermalErosion
{
    public void Apply(Mesh mesh, int vertexGridX, int vertexGridZ, in TerrainThermalErosionStepSettings s)
    {
        if (mesh == null || !TerrainHeightmapGrid.VertexCountMatches(mesh.vertexCount, vertexGridX, vertexGridZ))
            return;

        int width = vertexGridX;
        int depth = vertexGridZ;
        if (width < 2 || depth < 2 || s.iterations <= 0)
            return;

        var verts = mesh.vertices;
        float[,] heights = TerrainHeightmapGrid.FromVertices(verts, width, depth);

        RunThermal(heights, width, depth, s.iterations, s.talusHeightDelta, s.strength);

        TerrainHeightmapGrid.WriteYToVertices(verts, heights, width, depth);
        mesh.vertices = verts;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    private static void RunThermal(
        float[,] heights,
        int width,
        int depth,
        int iterations,
        float talusHeightDelta,
        float strength)
    {
        talusHeightDelta = Mathf.Max(1e-5f, talusHeightDelta);
        strength = Mathf.Clamp01(strength);

        var delta = new float[width, depth];

        for (int it = 0; it < iterations; it++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                    delta[x, z] = 0f;
            }

            void RelaxPair(int ax, int az, int bx, int bz)
            {
                float h0 = heights[ax, az];
                float h1 = heights[bx, bz];
                float diff = h0 - h1;
                if (diff <= talusHeightDelta)
                    return;
                float transfer = (diff - talusHeightDelta) * strength * 0.5f;
                delta[ax, az] -= transfer;
                delta[bx, bz] += transfer;
            }

            for (int x = 0; x < width - 1; x++)
            {
                for (int z = 0; z < depth; z++)
                    RelaxPair(x, z, x + 1, z);
            }

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth - 1; z++)
                    RelaxPair(x, z, x, z + 1);
            }

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                    heights[x, z] += delta[x, z];
            }
        }
    }
}
