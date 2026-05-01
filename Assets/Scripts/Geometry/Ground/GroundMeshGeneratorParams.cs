using UnityEngine;

/// <summary>
/// Parameters for <see cref="GroundMeshGenerator"/>. Use <see cref="Default"/> for a quick run,
/// or <see cref="WithRandomSeed"/> for varied noise without changing other fields.
/// </summary>
public struct GroundMeshGeneratorParams
{
    /// <summary>Horizontal extent along world X (meters).</summary>
    public float sizeX;

    /// <summary>Horizontal extent along world Z — "depth" of the patch (meters).</summary>
    public float sizeZ;

    /// <summary>Low end of final height after noise is normalized to 0–1.</summary>
    public float heightMin;

    /// <summary>High end of final height.</summary>
    public float heightMax;

    /// <summary>Quad columns (vertices = segmentsX + 1).</summary>
    public int segmentsX;

    /// <summary>Quad rows along Z.</summary>
    public int segmentsZ;

    /// <summary>Lower = broader, smoother hills (noise sampled in world space).</summary>
    public float noiseScale;

    /// <summary>FBM layers; more = finer detail (default-friendly).</summary>
    public int noiseOctaves;

    /// <summary>Amplitude falloff per octave (0–1).</summary>
    public float noisePersistence;

    /// <summary>Frequency multiplier per octave (&gt;1).</summary>
    public float noiseLacunarity;

    /// <summary>World-space shift of noise domain (use with seed for variation).</summary>
    public Vector3 noiseOffset;

    /// <summary>Derives extra domain jitter so runs differ even with same offset.</summary>
    public uint noiseSeed;

    public static GroundMeshGeneratorParams Default => new GroundMeshGeneratorParams
    {
        sizeX = 12f,
        sizeZ = 12f,
        heightMin = -0.2f,
        heightMax = 0.9f,
        segmentsX = 48,
        segmentsZ = 48,
        noiseScale = 0.11f,
        noiseOctaves = 4,
        noisePersistence = 0.48f,
        noiseLacunarity = 2.05f,
        noiseOffset = Vector3.zero,
        noiseSeed = 42u,
    };

    public GroundMeshGeneratorParams WithRandomSeed()
    {
        var p = this;
        p.noiseSeed = (uint)Random.Range(1, int.MaxValue);
        p.noiseOffset = new Vector3(Random.value * 733f, Random.value * 911f, Random.value * 577f);
        return p;
    }
}
