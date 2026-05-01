/// <summary>
/// Commands for procedural mesh generation (e.g. UI binders or DI).
/// </summary>
public interface IProceduralMeshController
{
    void Generate();
    void ReGenerate();
    void Export();
    void ExportMeshAsProjectAsset(string assetPath = null);
    void SetShapeById(string shapeId);
}
