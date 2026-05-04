using Geometry;
using UnityEngine;

namespace Meshes
{
    // mesh applier contract (unity side)
    public interface IMeshApplier
    {
        Mesh Apply(MeshData data);
    }
}