using UnityEngine;

namespace Geometry
{
    // vertex generator contract
    public interface IVertexGenerator
    {
        Vector3[] Generate(float size);
    }

    // triangle generator contract
    public interface ITriangleGenerator
    {
        int[] Generate();
    }

    // mesh builder contract (pure data)
    public interface IMeshDataGenerator
    {
        MeshData Generate(float size);
    }

    // mesh applier contract (unity side)
    public interface IMeshApplier
    {
        Mesh Apply(MeshData data);
    }

}