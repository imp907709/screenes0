using UnityEngine;

/// <summary>Settings for low-frequency continental mask / elevation (macro landmasses).</summary>
public readonly struct BiomeContinentalStepSettings
{
    public readonly uint noiseSeed;
    /// <summary>Approximate fBm cycles from one patch edge to the other (UV space); use ~1–4 for a few continents.</summary>
    public readonly float continentCyclesAcrossPatch;
    public readonly int continentOctaves;
    public readonly float persistence;
    public readonly float lacunarity;
    public readonly float heightMin;
    public readonly float heightMax;
    /// <summary>Noise threshold (0–1) separating ocean (low) from land (high).</summary>
    public readonly float seaLevel;
    /// <summary>Half-width of blended coast on the noise axis (0–1).</summary>
    public readonly float coastBlend;

    public BiomeContinentalStepSettings(
        uint noiseSeed,
        float continentCyclesAcrossPatch,
        int continentOctaves,
        float persistence,
        float lacunarity,
        float heightMin,
        float heightMax,
        float seaLevel,
        float coastBlend)
    {
        this.noiseSeed = noiseSeed;
        this.continentCyclesAcrossPatch = continentCyclesAcrossPatch;
        this.continentOctaves = continentOctaves;
        this.persistence = persistence;
        this.lacunarity = lacunarity;
        this.heightMin = heightMin;
        this.heightMax = heightMax;
        this.seaLevel = seaLevel;
        this.coastBlend = coastBlend;
    }
}

/// <summary>
/// Step 1 — continental shape: very low-frequency fBm in <b>normalized patch UV</b> (so scale is not tied to world meters),
/// clamped lacunarity for macro-only detail, then coast blend for ocean vs land read.
/// </summary>
public sealed class BiomeTerrainGenerationStep1ContinentalShape
{
    public void Apply(Mesh mesh, int gridX, int gridZ, in BiomeContinentalStepSettings s)
    {
        if (mesh == null || !TerrainHeightmapGrid.VertexCountMatches(mesh.vertexCount, gridX, gridZ))
            return;

        int oct = Mathf.Clamp(s.continentOctaves, 1, 3);
        float cycles = Mathf.Max(0.35f, s.continentCyclesAcrossPatch);
        float span = cycles * Mathf.PI * 2f;

        float pers = Mathf.Clamp(s.persistence, 0.32f, 0.58f);
        float lac = Mathf.Clamp(s.lacunarity, 1.5f, 2.15f);

        float invWm1 = gridX > 1 ? 1f / (gridX - 1f) : 0f;
        float invHm1 = gridZ > 1 ? 1f / (gridZ - 1f) : 0f;
        float sea = Mathf.Clamp01(s.seaLevel);
        float blend = Mathf.Max(0.02f, s.coastBlend);

        var verts = mesh.vertices;
        int w = gridX;
        int h = gridZ;
        for (int z = 0; z < h; z++)
        {
            for (int x = 0; x < w; x++)
            {
                int i = z * w + x;
                Vector3 v = verts[i];
                float u = gridX > 1 ? x * invWm1 : 0.5f;
                float vv = gridZ > 1 ? z * invHm1 : 0.5f;
                float nx = u * span;
                float nz = vv * span;
                float n = TerrainValueNoise.Fbm2D(nx, nz, oct, pers, lac, s.noiseSeed);
                n = Mathf.Clamp01(n);
                float landMask = Mathf.SmoothStep(sea - blend, sea + blend, n);
                v.y = Mathf.Lerp(s.heightMin, s.heightMax, landMask);
                verts[i] = v;
            }
        }

        mesh.vertices = verts;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
