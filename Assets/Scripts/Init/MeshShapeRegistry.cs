using System;
using System.Collections.Generic;

namespace Init
{
    /// <summary>
    /// One entry in the geometry dropdown: stable <see cref="Id"/>, UI <see cref="DisplayName"/>,
    /// and factory for the mesh binding (<see cref="CreateBinding"/>).
    /// </summary>
    public sealed class MeshShapeOption
    {
        private readonly Func<MeshShapeBinding> _createBinding;

        public MeshShapeOption(string id, string displayName, Func<MeshShapeBinding> createBinding)
        {
            Id = id;
            DisplayName = displayName;
            _createBinding = createBinding;
        }

        public string Id { get; }
        public string DisplayName { get; }

        public MeshShapeBinding CreateBinding() => _createBinding();
    }

    /// <summary>
    /// Authoritative list of shapes for UI and <see cref="ProceduralMeshController"/>.
    /// Add a row per shape using <see cref="MeshShapeIds"/> / <see cref="MeshShapeLabels"/>.
    /// </summary>
    public static class MeshShapeRegistry
    {
        public static IReadOnlyList<MeshShapeOption> All { get; } = new MeshShapeOption[]
        {
            new MeshShapeOption(MeshShapeIds.Sphere, MeshShapeLabels.Sphere, () => new SphereMeshShapeBinding()),
            new MeshShapeOption(MeshShapeIds.Ground, MeshShapeLabels.Ground, () => new GroundMeshShapeBinding()),
        };

        public static int IndexOfId(string id)
        {
            if (string.IsNullOrEmpty(id)) return -1;
            for (int i = 0; i < All.Count; i++)
            {
                if (string.Equals(All[i].Id, id, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            return -1;
        }

        public static bool TryGetById(string id, out MeshShapeOption option)
        {
            int i = IndexOfId(id);
            if (i < 0)
            {
                option = null;
                return false;
            }

            option = All[i];
            return true;
        }
    }
}
