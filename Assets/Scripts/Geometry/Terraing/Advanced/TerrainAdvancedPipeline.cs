using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Two grid inputs (vertices along X and Z) rebuild a flat XZ mesh; noise and erosion use that mesh. No manual vertex-count sync.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter))]
public class TerrainAdvancedPipeline : MonoBehaviour
{
    [Tooltip("Mesh vertex count along local X (≥2). Step 1 rebuilds the mesh to gridX×gridZ vertices.")]
    [FormerlySerializedAs("vertexGridX")]
    public int gridX = 11;

    [Tooltip("Mesh vertex count along local Z (≥2).")]
    [FormerlySerializedAs("vertexGridZ")]
    public int gridZ = 11;

    [Tooltip("If mesh is missing or vertex count ≠ gridX×gridZ, rebuild plane to match grid (edit + play enter).")]
    public bool autoRebuildPlaneWhenGridMismatch = true;

    [Header("Step 1 — Fractal noise (fBm)")]
    public uint noiseSeed = 42u;
    public int noiseOctaves = 5;
    public float noiseWorldScale = 0.18f;
    public float noisePersistence = 0.48f;
    public float noiseLacunarity = 2.05f;
    public float heightMin = -0.15f;
    public float heightMax = 1.2f;

    [Header("Step 2 — Hydraulic erosion")]
    public int hydraulicDroplets = 120_000;
    public int hydraulicMaxSteps = 48;
    [Range(0.05f, 0.95f)]
    public float hydraulicCarryStrength = 0.35f;

    [Header("Step 3 — Thermal (talus) erosion")]
    public int thermalIterations = 8;
    public float thermalTalusDelta = 0.04f;
    [Range(0.05f, 1f)]
    public float thermalStrength = 0.45f;

    private TerrainAdvancedGenerator _generator;

    private TerrainAdvancedGenerator Generator => _generator ??= new TerrainAdvancedGenerator();

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
        if (autoRebuildPlaneWhenGridMismatch)
            TryRebuildPlaneIfGridMismatch();
    }

    public void TryRebuildPlaneIfGridMismatch()
    {
        var mf = GetMeshFilter();
        if (mf == null)
            return;
        var m = mf.sharedMesh;
        if (m == null || !TerrainHeightmapGrid.VertexCountMatches(m.vertexCount, gridX, gridZ))
            RebuildMeshFromGrid();
    }

    /// <summary>Rebuilds <see cref="MeshFilter.mesh"/> as an XZ plane from <see cref="gridX"/> and <see cref="gridZ"/> only (see <see cref="TerrainAdvancedPlaneMeshFactory"/>).</summary>
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

    public void ApplyFractalNoiseToWorkingMesh()
    {
        var mesh = GetWorkingMesh();
        if (mesh == null)
            return;
        if (!ValidateGrid(mesh.vertexCount))
        {
            Debug.LogWarning(
                $"TerrainAdvancedPipeline: mesh vertex count {mesh.vertexCount} does not match grid {gridX}×{gridZ}. Run Step 1 to rebuild the mesh from the grid.");
            return;
        }

        var fractal = BuildFractalSettings();
        Generator.RunStep1FractalNoise(mesh, gridX, gridZ, in fractal);
    }

    public void ApplyStep1_FractalNoise()
    {
        if (RebuildMeshFromGrid() == null)
            return;
        ApplyFractalNoiseToWorkingMesh();
    }

    public void ApplyStep2_HydraulicErosion()
    {
        var mesh = GetWorkingMesh();
        if (mesh == null)
        {
            Debug.LogWarning("TerrainAdvancedPipeline: run Step 1 first (no mesh on MeshFilter).");
            return;
        }

        if (!ValidateGrid(mesh.vertexCount))
            return;

        var hydraulic = new TerrainHydraulicErosionStepSettings(
            Mathf.Max(0, hydraulicDroplets),
            Mathf.Max(1, hydraulicMaxSteps),
            hydraulicCarryStrength,
            noiseSeed ^ 0x9E3779B9u);

        Generator.RunStep2HydraulicErosion(mesh, gridX, gridZ, in hydraulic);
    }

    public void ApplyStep3_ThermalErosion()
    {
        var mesh = GetWorkingMesh();
        if (mesh == null)
        {
            Debug.LogWarning("TerrainAdvancedPipeline: run Step 1 first (no mesh on MeshFilter).");
            return;
        }

        if (!ValidateGrid(mesh.vertexCount))
            return;

        var thermal = new TerrainThermalErosionStepSettings(
            Mathf.Max(1, thermalIterations),
            thermalTalusDelta,
            thermalStrength);

        Generator.RunStep3ThermalErosion(mesh, gridX, gridZ, in thermal);
    }

    public void ApplyFullPipeline()
    {
        if (RebuildMeshFromGrid() == null)
            return;

        var mesh = GetWorkingMesh();
        if (mesh == null || !ValidateGrid(mesh.vertexCount))
            return;

        var fractal = BuildFractalSettings();
        var hydraulic = new TerrainHydraulicErosionStepSettings(
            Mathf.Max(0, hydraulicDroplets),
            Mathf.Max(1, hydraulicMaxSteps),
            hydraulicCarryStrength,
            noiseSeed ^ 0x9E3779B9u);
        var thermal = new TerrainThermalErosionStepSettings(
            Mathf.Max(1, thermalIterations),
            thermalTalusDelta,
            thermalStrength);

        Generator.RunFullPipeline(mesh, gridX, gridZ, in fractal, in hydraulic, in thermal);
    }

    private TerrainFractalNoiseStepSettings BuildFractalSettings() =>
        new TerrainFractalNoiseStepSettings(
            noiseSeed,
            noiseOctaves,
            noiseWorldScale,
            noisePersistence,
            noiseLacunarity,
            heightMin,
            heightMax);

    private Mesh GetWorkingMesh()
    {
        var mf = GetMeshFilter();
        if (mf == null)
            return null;
        return mf.sharedMesh;
    }

    private bool ValidateGrid(int vertexCount) =>
        TerrainHeightmapGrid.VertexCountMatches(vertexCount, gridX, gridZ);
}
