namespace Init
{
    /// <summary>
    /// Stable shape ids (wire to registry, bootstrap, saves). Add one const per shape.
    /// </summary>
    public static class MeshShapeIds
    {
        public const string Cube = "cube";
        public const string Sphere = "sphere";

        /// <summary>Bootstrap / UI default when nothing else applies.</summary>
        public const string Default = Sphere;
    }

    /// <summary>
    /// Dropdown and display labels. Add one const per shape (pair with <see cref="MeshShapeIds"/>).
    /// </summary>
    public static class MeshShapeLabels
    {
        public const string Cube = "Cube";
        public const string Sphere = "Sphere";
    }
}
