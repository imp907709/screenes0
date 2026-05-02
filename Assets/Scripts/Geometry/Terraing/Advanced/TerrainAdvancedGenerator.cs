using UnityEngine;

/// <summary>
/// Orchestrates terrain generation: calls <see cref="TerrainGenerationStep1FractalNoise"/>,
/// <see cref="TerrainGenerationStep2HydraulicErosion"/>, and <see cref="TerrainGenerationStep3ThermalErosion"/> in order.
/// </summary>
public sealed class TerrainAdvancedGenerator
{
    private readonly TerrainGenerationStep1FractalNoise _fractalStep = new TerrainGenerationStep1FractalNoise();
    private readonly TerrainGenerationStep2HydraulicErosion _hydraulicStep = new TerrainGenerationStep2HydraulicErosion();
    private readonly TerrainGenerationStep3ThermalErosion _thermalStep = new TerrainGenerationStep3ThermalErosion();

    public void RunStep1FractalNoise(Mesh mesh, int vertexGridX, int vertexGridZ, in TerrainFractalNoiseStepSettings settings) =>
        _fractalStep.Apply(mesh, vertexGridX, vertexGridZ, in settings);

    public void RunStep2HydraulicErosion(Mesh mesh, int vertexGridX, int vertexGridZ, in TerrainHydraulicErosionStepSettings settings) =>
        _hydraulicStep.Apply(mesh, vertexGridX, vertexGridZ, in settings);

    public void RunStep3ThermalErosion(Mesh mesh, int vertexGridX, int vertexGridZ, in TerrainThermalErosionStepSettings settings) =>
        _thermalStep.Apply(mesh, vertexGridX, vertexGridZ, in settings);

    /// <summary>Runs steps 1 → 2 → 3 on the same mesh.</summary>
    public void RunFullPipeline(
        Mesh mesh,
        int vertexGridX,
        int vertexGridZ,
        in TerrainFractalNoiseStepSettings fractal,
        in TerrainHydraulicErosionStepSettings hydraulic,
        in TerrainThermalErosionStepSettings thermal)
    {
        if (mesh == null)
            return;

        RunStep1FractalNoise(mesh, vertexGridX, vertexGridZ, in fractal);
        RunStep2HydraulicErosion(mesh, vertexGridX, vertexGridZ, in hydraulic);
        RunStep3ThermalErosion(mesh, vertexGridX, vertexGridZ, in thermal);
    }
}
