using UnityEngine;

public readonly struct TerrainFractalNoiseStepSettings
{
    public readonly uint noiseSeed;
    public readonly int noiseOctaves;
    public readonly float noiseWorldScale;
    public readonly float noisePersistence;
    public readonly float noiseLacunarity;
    public readonly float heightMin;
    public readonly float heightMax;

    public TerrainFractalNoiseStepSettings(
        uint noiseSeed,
        int noiseOctaves,
        float noiseWorldScale,
        float noisePersistence,
        float noiseLacunarity,
        float heightMin,
        float heightMax)
    {
        this.noiseSeed = noiseSeed;
        this.noiseOctaves = noiseOctaves;
        this.noiseWorldScale = noiseWorldScale;
        this.noisePersistence = noisePersistence;
        this.noiseLacunarity = noiseLacunarity;
        this.heightMin = heightMin;
        this.heightMax = heightMax;
    }
}

/// <summary>Step 1: multi-octave (fBm) height on a regular grid mesh using hash value noise.</summary>
public sealed class TerrainGenerationStep1FractalNoise
{
    public void Apply(Mesh mesh, int vertexGridX, int vertexGridZ, in TerrainFractalNoiseStepSettings s)
    {
        if (mesh == null || !TerrainHeightmapGrid.VertexCountMatches(mesh.vertexCount, vertexGridX, vertexGridZ))
            return;

        var verts = mesh.vertices;
        int w = vertexGridX;
        for (int z = 0; z < vertexGridZ; z++)
        {
            for (int x = 0; x < w; x++)
            {
                int i = z * w + x;
                Vector3 v = verts[i];
                float n = TerrainValueNoise.Fbm2D(
                    v.x * s.noiseWorldScale,
                    v.z * s.noiseWorldScale,
                    s.noiseOctaves,
                    s.noisePersistence,
                    s.noiseLacunarity,
                    s.noiseSeed);
                n = Mathf.Clamp01(n);
                v.y = Mathf.Lerp(s.heightMin, s.heightMax, n);
                verts[i] = v;
            }
        }

        mesh.vertices = verts;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
