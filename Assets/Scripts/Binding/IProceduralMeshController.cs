namespace Binding
{
    /// <summary>
    /// Commands for procedural mesh generation (e.g. UI binders or DI).
    /// </summary>
    public interface IProceduralMeshController
    {
        void Generate();
        void ReGenerate();
        void Export();
        /// <param name="exportFileName">Mesh base name only (no folder, no .asset); null uses shape default.</param>
        void ExportMeshAsProjectAsset(string exportFileName = null);
        void SetShapeById(string shapeId);
    }
}
