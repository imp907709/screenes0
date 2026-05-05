using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Meshes.GeneralMesh;
using UnityEngine;

/// <summary>
/// One place: sample biomes, grid world, seeds, then <see cref="WorldGenerator"/> + <see cref="OctagonMeshBuilder"/>.
/// </summary>
public static class WrapperGenerator
{
    public const int DefaultGridWidth = 12;
    public const int DefaultGridDepth = 12;
    public const float DefaultCellSpacing = 1f;

    /// <summary>Three biomes tuned for visible spread and height contrast.</summary>
    public static List<Biome> CreateSampleBiomes()
    {
        return new List<Biome>
        {
            new Biome
            {
                Id = 0,
                Name = "Ocean",
                SpreadSpeed = 1.2f,
                Strength = 1f,
                Chaos = 0.35f,
                HeightBias = -0.6f,
                Color = new Color(0.15f, 0.35f, 0.75f, 1f),
            },
            new Biome
            {
                Id = 1,
                Name = "Plains",
                SpreadSpeed = 1f,
                Strength = 0.95f,
                Chaos = 0.45f,
                HeightBias = 0f,
                Color = new Color(0.35f, 0.65f, 0.25f, 1f),
            },
            new Biome
            {
                Id = 2,
                Name = "Mountains",
                SpreadSpeed = 0.75f,
                Strength = 1.05f,
                Chaos = 0.55f,
                HeightBias = 1.1f,
                Color = new Color(0.55f, 0.45f, 0.38f, 1f),
            },
        };
    }

    /// <summary>Row-major index: <c>x + z * width</c>.</summary>
    public static int CellIndex(int x, int z, int width) => x + z * width;

    /// <summary>Build flat grid of cells with 8-neighbor links.</summary>
    public static World BuildSampleGrid(int width, int depth, float spacing)
    {
        var world = new World();
        var grid = new Cell[width, depth];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                var cell = new Cell { Position = new Vector3(x * spacing, 0f, z * spacing) };
                grid[x, z] = cell;
                world.Cells.Add(cell);
            }
        }

        void Link(int x, int z, int nx, int nz)
        {
            if (nx < 0 || nx >= width || nz < 0 || nz >= depth)
                return;
            grid[x, z].Neighbors.Add(grid[nx, nz]);
        }

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Link(x, z, x - 1, z);
                Link(x, z, x + 1, z);
                Link(x, z, x, z - 1);
                Link(x, z, x, z + 1);
                Link(x, z, x - 1, z - 1);
                Link(x, z, x + 1, z - 1);
                Link(x, z, x - 1, z + 1);
                Link(x, z, x + 1, z + 1);
            }
        }

        return world;
    }

    /// <summary>Corner + center seeds so all three biomes get territory.</summary>
    public static List<BiomeSeed> CreateSampleSeeds(World world, IReadOnlyList<Biome> biomes, int width, int depth)
    {
        if (biomes == null || biomes.Count < 3)
        {
            Debug.LogError("WrapperGenerator.CreateSampleSeeds: need at least 3 biomes.");
            return new List<BiomeSeed>();
        }

        int cx = Mathf.Clamp(width / 2, 0, width - 1);
        int cz = Mathf.Clamp(depth / 2, 0, depth - 1);

        return new List<BiomeSeed>
        {
            new BiomeSeed { Biome = biomes[0], StartCell = world.Cells[CellIndex(0, 0, width)] },
            new BiomeSeed { Biome = biomes[1], StartCell = world.Cells[CellIndex(width - 1, 0, width)] },
            new BiomeSeed { Biome = biomes[2], StartCell = world.Cells[CellIndex(cx, cz, width)] },
        };
    }

    /// <summary>Biome spread, regions, height — full pipeline.</summary>
    public static void RunWorldGenerators(World world, List<BiomeSeed> seeds)
    {
        WorldGenerator.Generate(world, seeds);
    }

    public static Mesh BuildWorldMesh(World world)
    {
        return OctagonMeshBuilder.BuildWithBiomeColors(world);
    }

    /// <summary>Sample world only (generators applied). Optional <paramref name="randomSeed"/> for reproducible noise.</summary>
    public static World GenerateSampleWorld(
        int width = DefaultGridWidth,
        int depth = DefaultGridDepth,
        float spacing = DefaultCellSpacing,
        int? randomSeed = null)
    {
        if (randomSeed.HasValue)
            Random.InitState(randomSeed.Value);

        List<Biome> biomes = CreateSampleBiomes();
        World world = BuildSampleGrid(width, depth, spacing);
        List<BiomeSeed> seeds = CreateSampleSeeds(world, biomes, width, depth);
        RunWorldGenerators(world, seeds);
        return world;
    }

    /// <summary>World + octagon mesh in one call.</summary>
    public static (World world, Mesh mesh) GenerateSampleWorldWithMesh(
        int width = DefaultGridWidth,
        int depth = DefaultGridDepth,
        float spacing = DefaultCellSpacing,
        int? randomSeed = null)
    {
        World world = GenerateSampleWorld(width, depth, spacing, randomSeed);
        Mesh mesh = BuildWorldMesh(world);
        return (world, mesh);
    }

    /// <summary>
    /// Scene step: new GameObject with <see cref="MeshFilter"/> / <see cref="MeshRenderer"/> and the given mesh.
    /// Does not run generators — call after <see cref="BuildWorldMesh"/> or <see cref="GenerateSampleWorldWithMesh"/>.
    /// </summary>
    public static GameObject SpawnSampleWorldMeshInScene(Mesh mesh, string objectName = "BiomeSampleWorld")
    {
        if (mesh == null || mesh.vertexCount == 0)
        {
            Debug.LogError("WrapperGenerator.SpawnSampleWorldMeshInScene: mesh is null or empty.");
            return null;
        }

        var go = new GameObject(objectName);
#if UNITY_EDITOR
        Undo.RegisterCreatedObjectUndo(go, objectName);
#endif

        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();
        mf.sharedMesh = mesh;

        Material mat = MaterialFactory.GetBiomeVertexColorMaterial();
        if (mat == null)
        {
            mat = MaterialFactory.GetDefault();
            if (mat == null)
            {
                Debug.LogError("WrapperGenerator.SpawnSampleWorldMeshInScene: no material available.");
#if UNITY_EDITOR
                Undo.DestroyObjectImmediate(go);
#else
                Object.Destroy(go);
#endif
                return null;
            }

            Debug.LogWarning("WrapperGenerator.SpawnSampleWorldMeshInScene: vertex-color shader missing; using default material (biome tint may not show).");
        }

        mr.sharedMaterial = mat;

#if UNITY_EDITOR
        if (Selection.activeTransform != null)
            go.transform.SetPositionAndRotation(Selection.activeTransform.position, Selection.activeTransform.rotation);
        else
            go.transform.position = Vector3.zero;

        Selection.activeGameObject = go;
#else
        go.transform.position = Vector3.zero;
#endif

        return go;
    }
}
