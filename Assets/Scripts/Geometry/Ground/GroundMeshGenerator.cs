using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Procedural XZ ground mesh with FBM noise (pseudo-3D Perlin mix) and sensible defaults.
/// </summary>
public class GroundMeshGenerator : IMeshGenerator
{
    private GroundMeshGeneratorParams _template;

    public GroundMeshGenerator(GroundMeshGeneratorParams template = default)
    {
        _template = template.sizeX > 0.001f && template.sizeZ > 0.001f
            ? template
            : GroundMeshGeneratorParams.Default;
    }

    /// <summary>Quick mesh using <see cref="GroundMeshGeneratorParams.Default"/> (optionally randomize first).</summary>
    public Mesh Generate(bool randomizeSeed = false)
    {
        var p = randomizeSeed ? GroundMeshGeneratorParams.Default.WithRandomSeed() : GroundMeshGeneratorParams.Default;
        return Generate(p);
    }

    /// <summary>
    /// <see cref="IMeshGenerator"/>: uses template from ctor; <paramref name="size"/> sets uniform X/Z footprint when &gt; 0.
    /// </summary>
    public Mesh Generate(float size)
    {
        var p = _template;
        if (size > 0.001f)
        {
            p.sizeX = size;
            p.sizeZ = size;
        }

        return Generate(p);
    }

    /// <summary>Full control via params.</summary>
    public Mesh Generate(GroundMeshGeneratorParams p)
    {
        int sx = Mathf.Max(2, p.segmentsX);
        int sz = Mathf.Max(2, p.segmentsZ);
        int oct = Mathf.Clamp(p.noiseOctaves, 1, 8);

        float halfX = p.sizeX * 0.5f;
        float halfZ = p.sizeZ * 0.5f;

        float seedJitter = Hash01(p.noiseSeed);
        Vector3 off = p.noiseOffset + new Vector3(seedJitter * 50f, Hash01(p.noiseSeed * 7u) * 50f, Hash01(p.noiseSeed * 13u) * 50f);

        int vx = sx + 1;
        int vz = sz + 1;
        var vertices = new Vector3[vx * vz];
        var uvs = new Vector2[vx * vz];
        var heights = new float[vx * vz];

        float hMin = float.PositiveInfinity;
        float hMax = float.NegativeInfinity;

        for (int z = 0; z < vz; z++)
        {
            float tz = z / (float)sz;
            float worldZ = Mathf.Lerp(-halfZ, halfZ, tz);

            for (int x = 0; x < vx; x++)
            {
                float tx = x / (float)sx;
                float worldX = Mathf.Lerp(-halfX, halfX, tx);

                float n = FbmPseudo3D(worldX, worldZ, off, p.noiseScale, oct, p.noisePersistence, p.noiseLacunarity);
                heights[z * vx + x] = n;
                hMin = Mathf.Min(hMin, n);
                hMax = Mathf.Max(hMax, n);
            }
        }

        float denom = Mathf.Max(1e-5f, hMax - hMin);

        for (int z = 0; z < vz; z++)
        {
            for (int x = 0; x < vx; x++)
            {
                int i = z * vx + x;
                float tz = z / (float)sz;
                float tx = x / (float)sx;
                float worldX = Mathf.Lerp(-halfX, halfX, tx);
                float worldZ = Mathf.Lerp(-halfZ, halfZ, tz);

                float t = (heights[i] - hMin) / denom;
                float y = Mathf.Lerp(p.heightMin, p.heightMax, t);

                vertices[i] = new Vector3(worldX, y, worldZ);
                uvs[i] = new Vector2(tx, tz);
            }
        }

        var triangles = new List<int>(sx * sz * 6);
        for (int z = 0; z < sz; z++)
        {
            for (int x = 0; x < sx; x++)
            {
                int i0 = z * vx + x;
                int i1 = i0 + 1;
                int i2 = i0 + vx;
                int i3 = i2 + 1;

                triangles.Add(i0);
                triangles.Add(i2);
                triangles.Add(i1);
                triangles.Add(i1);
                triangles.Add(i2);
                triangles.Add(i3);
            }
        }

        var mesh = new Mesh();
        mesh.name = "GeneratedGround";
        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    /// <summary>Fractional Brownian motion using a Perlin mix for a 3D-ish feel.</summary>
    private static float FbmPseudo3D(float wx, float wz, Vector3 off, float baseScale, int octaves, float persistence, float lacunarity)
    {
        float sum = 0f;
        float amp = 1f;
        float freq = 1f;
        float norm = 0f;

        for (int o = 0; o < octaves; o++)
        {
            float s = baseScale * freq;
            float nx = wx * s + off.x;
            float nz = wz * s + off.z;
            float ny = (wx * 0.37f + wz * 0.61f) * s + off.y;

            float a = Mathf.PerlinNoise(nx + ny * 0.31f, nz - ny * 0.27f);
            float b = Mathf.PerlinNoise(nz + ny * 0.41f, nx - ny * 0.19f);
            float c = Mathf.PerlinNoise(nx - nz * 0.22f + ny * 0.13f, nx * 0.29f + nz * 0.17f);
            float n = (a + b + c) / 3f;

            sum += n * amp;
            norm += amp;
            amp *= persistence;
            freq *= lacunarity;
        }

        return norm > 1e-5f ? sum / norm : 0f;
    }

    private static float Hash01(uint seed)
    {
        seed ^= seed << 13;
        seed ^= seed >> 17;
        seed ^= seed << 5;
        return (seed & 0xFFFF) / 65535f;
    }
}
