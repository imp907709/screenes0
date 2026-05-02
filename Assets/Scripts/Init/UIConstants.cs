namespace Init
{
    public static class UIConstants
    {
        public const string GenerateObjTag = "generateObj";
        public const string GenerateObjButtonName = "generateObjButton";
        public const string ReGenerateObjButtonName = "reGenerateObjButton";
        public const string ExportMeshButtonName = "exportMeshObjButton";
        public const string GeometrySelectorName = "geometrySelector";
    }

    public static class PathConstants
    {
        /// <summary>Unity asset path for generated mesh assets (no trailing slash).</summary>
        public const string MeshAssetFolder = "Assets/GeneratedMeshes";

        public const string MeshAssetExtension = ".asset";
    }
}
