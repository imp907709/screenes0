using System;
using Binding.UI;
using Core;
using Geometry.Cube;
using Geometry.Ground;
using Geometry.Sphere;

namespace Binding
{
    /// <summary>
    /// Serializable strategy for mesh shape, generator, and export defaults.
    /// Assign a concrete type via the SerializeReference picker on <see cref="ProceduralMeshController"/>,
    /// or drive it from <see cref="Init.MeshShapeRegistry"/> / UI (e.g. <see cref="GeometrySelectorBinder"/>).
    /// </summary>
    [Serializable]
    public abstract class MeshShapeBinding
    {
        public abstract IMeshGenerator CreateGenerator();
        public abstract string RootObjectName { get; }
        public abstract string ObjFileBaseName { get; }
    }

    [Serializable]
    public sealed class CubeMeshShapeBinding : MeshShapeBinding
    {
        public override IMeshGenerator CreateGenerator() => new CubeMeshGenerator();
        public override string RootObjectName => "GeneratedCube";
        public override string ObjFileBaseName => "cube";
    }

    [Serializable]
    public sealed class SphereMeshShapeBinding : MeshShapeBinding
    {
        public override IMeshGenerator CreateGenerator() => new SphereMeshGenerator();
        public override string RootObjectName => "GeneratedSphere";
        public override string ObjFileBaseName => "sphere";
    }

    [Serializable]
    public sealed class GroundMeshShapeBinding : MeshShapeBinding
    {
        public override IMeshGenerator CreateGenerator() => new GroundMeshGenerator();
        public override string RootObjectName => "GeneratedGround";
        public override string ObjFileBaseName => "ground";
    }
}