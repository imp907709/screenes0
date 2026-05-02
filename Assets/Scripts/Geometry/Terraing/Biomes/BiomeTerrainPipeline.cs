using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Biome terrain: drop on a <see cref="MeshFilter"/> object, set <see cref="gridX"/>/<see cref="gridZ"/>, run steps (continental first).
/// Reuses plane mesh factory and heightmap layout from Advanced terrain.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter))]
public class BiomeTerrainPipeline : MonoBehaviour
{
    [Tooltip("Mesh vertex count along local X (≥2).")]
    public int gridX = 32;

    [Tooltip("Mesh vertex count along local Z (≥2).")]
    public int gridZ = 32;

    [Tooltip("If the MeshFilter mesh is missing or its vertex count ≠ gridX×gridZ, rebuild a flat plane to match the grid (edit mode + play enter).")]
    public bool autoRebuildPlaneWhenGridMismatch = true;

    [Header("Step 1 — Continental shape (macro)")]
    public uint noiseSeed = 1u;
    [Tooltip("How many continent-scale rises across the patch edge (~1–3 = a few landmasses; higher = busier). Uses UV space, not world meters.")]
    [FormerlySerializedAs("continentScale")]
    public float continentCyclesAcrossPatch = 2.2f;
    [Range(1, 3)]
    public int continentOctaves = 2;
    [Range(0.1f, 0.95f)]
    public float continentPersistence = 0.48f;
    [Tooltip("Clamped inside the continental step (~1.5–2.15) so huge values cannot turn this into high-frequency noise.")]
    public float continentLacunarity = 2f;
    [Range(0.05f, 0.95f)]
    [Tooltip("Noise value below ≈ ocean, above ≈ land (0–1 on fBm output).")]
    public float seaLevel = 0.45f;
    [Range(0.02f, 0.35f)]
    [Tooltip("Width of blended coast (noise space).")]
    public float coastBlend = 0.16f;
    public float heightMin = -0.35f;
    public float heightMax = 1.1f;

    private BiomeTerrainGenerator _generator;

    private BiomeTerrainGenerator Generator => _generator ??= new BiomeTerrainGenerator();

    public MeshFilter GetMeshFilter() => GetComponent<MeshFilter>();

    private void Awake()
    {
        if (autoRebuildPlaneWhenGridMismatch)
            TryRebuildPlaneIfGridMismatch();
    }

    private void OnValidate()
    {
        gridX = Mathf.Max(2, gridX);
        gridZ = Mathf.Max(2, gridZ);
        continentCyclesAcrossPatch = Mathf.Max(0.25f, continentCyclesAcrossPatch);
        coastBlend = Mathf.Max(0.02f, coastBlend);
        if (autoRebuildPlaneWhenGridMismatch)
            TryRebuildPlaneIfGridMismatch();
    }

    /// <summary>Rebuilds the plane when <see cref="MeshFilter.sharedMesh"/> is null or vertex count does not equal <see cref="gridX"/>×<see cref="gridZ"/>.</summary>
    public void TryRebuildPlaneIfGridMismatch()
    {
        var mf = GetMeshFilter();
        if (mf == null)
            return;

        var m = mf.sharedMesh;
        if (m == null || !TerrainHeightmapGrid.VertexCountMatches(m.vertexCount, gridX, gridZ))
            RebuildMeshFromGrid();
    }

    public Mesh RebuildMeshFromGrid()
    {
        var mf = GetMeshFilter();
        if (mf == null)
            return null;

        gridX = Mathf.Max(2, gridX);
        gridZ = Mathf.Max(2, gridZ);
        var mesh = TerrainAdvancedPlaneMeshFactory.CreateXZPlane(gridX, gridZ);
        mf.mesh = mesh;
        return mesh;
    }

    public void ApplyContinentalToWorkingMesh()
    {
        var mesh = GetWorkingMesh();
        if (mesh == null)
            return;
        if (!TerrainHeightmapGrid.VertexCountMatches(mesh.vertexCount, gridX, gridZ))
        {
            Debug.LogWarning(
                $"BiomeTerrainPipeline: mesh has {mesh.vertexCount} verts but grid is {gridX}×{gridZ}. Enable Auto Rebuild Plane When Grid Mismatch or call RebuildMeshFromGrid / Step 1.");
            return;
        }

        var settings = BuildContinentalSettings();
        Generator.RunStep1ContinentalShape(mesh, gridX, gridZ, in settings);
    }

    public void ApplyStep1_ContinentalShape()
    {
        if (RebuildMeshFromGrid() == null)
            return;
        ApplyContinentalToWorkingMesh();
    }

    private BiomeContinentalStepSettings BuildContinentalSettings() =>
        new BiomeContinentalStepSettings(
            noiseSeed,
            continentCyclesAcrossPatch,
            continentOctaves,
            continentPersistence,
            continentLacunarity,
            heightMin,
            heightMax,
            seaLevel,
            coastBlend);

    private Mesh GetWorkingMesh()
    {
        var mf = GetMeshFilter();
        return mf != null ? mf.sharedMesh : null;
    }
}
