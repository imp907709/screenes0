using System;
using UnityEngine;

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
    public abstract string DefaultProjectAssetPath { get; }
}

[Serializable]
public sealed class CubeMeshShapeBinding : MeshShapeBinding
{
    public override IMeshGenerator CreateGenerator() => new CubeMeshGenerator();
    public override string RootObjectName => "GeneratedCube";
    public override string ObjFileBaseName => "cube";
    public override string DefaultProjectAssetPath => "GeneratedMeshes/GeneratedCube.asset";
}

[Serializable]
public sealed class SphereMeshShapeBinding : MeshShapeBinding
{
    public override IMeshGenerator CreateGenerator() => new SphereMeshGenerator();
    public override string RootObjectName => "GeneratedSphere";
    public override string ObjFileBaseName => "sphere";
    public override string DefaultProjectAssetPath => "GeneratedMeshes/GeneratedSphere.asset";
}
