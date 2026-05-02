using UnityEngine;

/// <summary>Deterministic 2D value noise + fBm (no Unity Perlin).</summary>
public static class TerrainValueNoise
{
    public static float Fbm2D(float x, float z, int octaves, float persistence, float lacunarity, uint seed)
    {
        float sum = 0f;
        float amp = 1f;
        float freq = 1f;
        float norm = 0f;
        octaves = Mathf.Clamp(octaves, 1, 10);

        for (int o = 0; o < octaves; o++)
        {
            sum += ValueNoise2D(x * freq + seed * 0.001f, z * freq + seed * 0.002f, seed + (uint)o * 7919u) * amp;
            norm += amp;
            amp *= persistence;
            freq *= lacunarity;
        }

        return norm > 1e-5f ? sum / norm : 0f;
    }

    public static float ValueNoise2D(float x, float z, uint seed)
    {
        int x0 = Mathf.FloorToInt(x);
        int z0 = Mathf.FloorToInt(z);
        float fx = x - x0;
        float fz = z - z0;
        float u = fx * fx * (3f - 2f * fx);
        float v = fz * fz * (3f - 2f * fz);

        float a = Hash01(x0, z0, seed);
        float b = Hash01(x0 + 1, z0, seed);
        float c = Hash01(x0, z0 + 1, seed);
        float d = Hash01(x0 + 1, z0 + 1, seed);

        return Mathf.Lerp(Mathf.Lerp(a, b, u), Mathf.Lerp(c, d, u), v);
    }

    private static float Hash01(int x, int z, uint seed)
    {
        uint n = (uint)x * 374761393u ^ (uint)z * 668265263u ^ seed * 2246822519u;
        n = (n ^ (n >> 13)) * 1274126177u;
        n ^= n >> 16;
        return (n & 0xFFFFFF) / 16777215f;
    }
}
