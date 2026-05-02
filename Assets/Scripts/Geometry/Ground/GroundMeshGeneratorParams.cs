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

    /// <summary>Add to mesh local X before sampling (chunk origin for tiling).</summary>
    public float worldOriginX;

    /// <summary>Add to mesh local Z before sampling.</summary>
    public float worldOriginZ;

    /// <summary>If true, only original single FBM + per-mesh min/max normalization.</summary>
    public bool useLegacyFbmOnly;

    /// <summary>Large-scale 0–1 field; leave 0 to auto-scale from <see cref="noiseScale"/>.</summary>
    public float zoneNoiseScale;

    public int zoneOctaves;
    public float zonePersistence;
    public float zoneLacunarity;

    public float plainNoiseScaleMul;
    public int plainOctaves;
    public float plainPersistence;

    public float hillNoiseScaleMul;
    public int hillOctaves;
    public float hillPersistence;

    public float mountainNoiseScaleMul;
    public int mountainOctaves;
    public float mountainPersistence;

    public float blendPlainsToHillsStart;
    public float blendPlainsToHillsEnd;
    public float blendHillsToMountainsStart;
    public float blendHillsToMountainsEnd;

    public float domainWarpAmplitude;
    public float domainWarpNoiseScale;

    public float interiorCarveAmount;
    public float interiorCarveNoiseScale;
    public float interiorCarveThreshold;
    public float interiorCarveSoftness;

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
        worldOriginX = 0f,
        worldOriginZ = 0f,
        useLegacyFbmOnly = false,
        zoneNoiseScale = 0f,
        zoneOctaves = 3,
        zonePersistence = 0.52f,
        zoneLacunarity = 2f,
        plainNoiseScaleMul = 0.62f,
        plainOctaves = 3,
        plainPersistence = 0.58f,
        hillNoiseScaleMul = 1f,
        hillOctaves = 4,
        hillPersistence = 0.48f,
        mountainNoiseScaleMul = 1.38f,
        mountainOctaves = 5,
        mountainPersistence = 0.42f,
        blendPlainsToHillsStart = 0.26f,
        blendPlainsToHillsEnd = 0.52f,
        blendHillsToMountainsStart = 0.48f,
        blendHillsToMountainsEnd = 0.84f,
        domainWarpAmplitude = 1.1f,
        domainWarpNoiseScale = 0.045f,
        interiorCarveAmount = 0.22f,
        interiorCarveNoiseScale = 0.18f,
        interiorCarveThreshold = 0.55f,
        interiorCarveSoftness = 0.22f,
    };

    public GroundMeshGeneratorParams WithRandomSeed()
    {
        var p = this;
        p.noiseSeed = (uint)Random.Range(1, int.MaxValue);
        p.noiseOffset = new Vector3(Random.value * 733f, Random.value * 911f, Random.value * 577f);
        return p;
    }
}
