using UnityEngine;

/// <summary>Orchestrates biome terrain steps on one mesh (starts with continental macro shape).</summary>
public sealed class BiomeTerrainGenerator
{
    private readonly BiomeTerrainGenerationStep1ContinentalShape _continentalStep = new BiomeTerrainGenerationStep1ContinentalShape();

    public void RunStep1ContinentalShape(Mesh mesh, int gridX, int gridZ, in BiomeContinentalStepSettings settings) =>
        _continentalStep.Apply(mesh, gridX, gridZ, in settings);
}
