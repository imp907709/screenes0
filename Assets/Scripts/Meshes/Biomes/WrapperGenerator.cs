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

    /// <summary>Plains / forest get similar spread so they fight; chaos pass then muddles their border.</summary>
    public static List<Biome> CreateSampleBiomes()
    {
        return new List<Biome>
        {
            new Biome
            {
                Id = 0,
                Name = "Plains",
                SpreadSpeed = 1.05f,
                Strength = 0.95f,
                Chaos = 0.1f,
                HeightBias = 0.0f,
                Color = new Color(0.58f, 0.86f, 0.48f, 1f),
            },
            new Biome
            {
                Id = 1,
                Name = "Forest",
                SpreadSpeed = 1.02f,
                Strength = 0.96f,
                Chaos = 0.1f,
                HeightBias = 0.0f,
                Color = new Color(0.1f, 0.38f, 0.16f, 1f),
            },
            new Biome
            {
                Id = 2,
                Name = "Tundra",
                SpreadSpeed = 0.92f,
                Strength = 0.9f,
                Chaos = 0.42f,
                HeightBias = 0.0f,
                Color = new Color(0.94f, 0.86f, 0.38f, 1f),
            },
            new Biome
            {
                Id = 3,
                Name = "Hills",
                SpreadSpeed = 0.88f,
                Strength = 0.98f,
                Chaos = 0.38f,
                HeightBias = 0.0f,
                Color = new Color(0.52f, 0.38f, 0.26f, 1f),
            },
            new Biome
            {
                Id = 4,
                Name = "Mountains",
                SpreadSpeed = 0.72f,
                Strength = 0.95f,
                Chaos = 0.48f,
                HeightBias = 0.00f,
                Color = new Color(0.48f, 0.49f, 0.52f, 1f),
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
    
    public static World BuildSampleHexGrid(int width, int depth, float spacing)
    {
        var world = new World();
        var grid = new Cell[width, depth];

        float size = spacing;

        float hexWidth = size * 2f;
        float hexHeight = Mathf.Sqrt(3f) * size;

        // -------------------------
        // 1. CREATE CELLS (HEX LAYOUT)
        // -------------------------
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                float xPos = x * hexWidth * 0.75f;
                float zPos = z * hexHeight + (x % 2 == 0 ? 0f : hexHeight / 2f);

                var cell = new Cell
                {
                    Position = new Vector3(xPos, 0f, zPos)
                };

                grid[x, z] = cell;
                world.Cells.Add(cell);
            }
        }

        // -------------------------
        // 2. HEX NEIGHBOR OFFSETS
        // -------------------------
        (int dx, int dz)[] dirs =
        {
            (+1,  0),
            (-1,  0),
            (0, +1),
            (0, -1),
            (+1, -1),
            (-1, +1)
        };

        // -------------------------
        // 3. LINK NEIGHBORS (6-WAY)
        // -------------------------
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
                foreach (var d in dirs)
                {
                    Link(x, z, x + d.dx, z + d.dz);
                }
            }
        }

        return world;
    }

    /// <summary>Seeds spread out + plains/forest starts adjacent (diagonal) so their fronts collide early.</summary>
    public static List<BiomeSeed> CreateSampleSeeds2(World world, IReadOnlyList<Biome> biomes, int width, int depth, int required = 5)
    {
        if (biomes == null || biomes.Count < required)
        {
            Debug.LogError($"WrapperGenerator.CreateSampleSeeds: need at least {required} biomes (Plains, Forest, Tundra, Hills, Mountains).");
            return new List<BiomeSeed>();
        }

        int cx = Mathf.Clamp(width / 2, 0, width - 1);
        int cz = Mathf.Clamp(depth / 2, 0, depth - 1);
        int px = Mathf.Clamp(width / 4, 0, width - 1);
        int pz = Mathf.Clamp(depth / 4, 0, depth - 1);
        int fx = Mathf.Clamp(px + 1, 0, width - 1);
        int fz = Mathf.Clamp(pz + 1, 0, depth - 1);

        return new List<BiomeSeed>
        {
            new BiomeSeed { Biome = biomes[0], StartCell = world.Cells[CellIndex(px, pz, width)] },
            new BiomeSeed { Biome = biomes[1], StartCell = world.Cells[CellIndex(fx, fz, width)] },
            new BiomeSeed { Biome = biomes[2], StartCell = world.Cells[CellIndex(0, depth - 1, width)] },
            new BiomeSeed { Biome = biomes[3], StartCell = world.Cells[CellIndex(width - 1, 0, width)] },
            new BiomeSeed { Biome = biomes[4], StartCell = world.Cells[CellIndex(cx, cz, width)] },
        };
    }
    
    public static List<BiomeSeed> CreateSampleSeeds(
    World world,
    IReadOnlyList<Biome> biomes,
    int width,
    int depth,
    int required = 5)
    {
        if (biomes == null || biomes.Count < required)
        {
            Debug.LogError($"CreateSampleSeeds: need at least {required} biomes.");
            return new List<BiomeSeed>();
        }

        // expected order (you control this outside)
        var plains     = biomes[0];
        var forest     = biomes[1];
        var desert     = biomes[2];
        var hills      = biomes[3];
        var mountains  = biomes[4];

        var seeds = new List<BiomeSeed>();

        int cx = width / 2;
        int cz = depth / 2;

        // ------------------------
        // helpers
        int rx(int min, int max) => UnityEngine.Random.Range(min, max);
        int rz(int min, int max) => UnityEngine.Random.Range(min, max);

        BiomeSeed Create(Biome biome, int x, int z)
        {
            return new BiomeSeed
            {
                Biome = biome,
                StartCell = world.Cells[CellIndex(
                    Mathf.Clamp(x, 0, width - 1),
                    Mathf.Clamp(z, 0, depth - 1),
                    width)]
            };
        }

        // ------------------------
        // CENTER (forest + plains, multiple seeds)

        int centerRadiusX = width / 4;
        int centerRadiusZ = depth / 4;

        int centerCount = 3; // tweak

        for (int i = 0; i < centerCount; i++)
        {
            int x = cx + rx(-centerRadiusX, centerRadiusX);
            int z = cz + rz(-centerRadiusZ, centerRadiusZ);

            // alternate forest / plains
            var biome = (i % 2 == 0) ? forest : plains;

            seeds.Add(Create(biome, x, z));
        }

        // ------------------------
        // SOUTH (desert)

        int southCount = 2;

        for (int i = 0; i < southCount; i++)
        {
            int x = rx(0, width);
            int z = rz(0, depth / 3); // bottom third

            seeds.Add(Create(desert, x, z));
        }

        // ------------------------
        // NORTH (hills + mountains)

        int northCount = 2;

        for (int i = 0; i < northCount; i++)
        {
            int x = rx(0, width);
            int z = rz(depth * 2 / 3, depth); // top third

            var biome = (i % 2 == 0) ? hills : mountains;

            seeds.Add(Create(biome, x, z));
        }

        return seeds;
    }
        

    /// <summary>Biome spread, optional plains↔forest chaos, regions, height.</summary>
    public static void RunWorldGenerators(World world, List<BiomeSeed> seeds, Biome plainsForChaos = null, Biome forestForChaos = null)
    {
        WorldGenerator.Generate(world, seeds, plainsForChaos, forestForChaos);
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
        World world = BuildSampleHexGrid(width, depth, spacing);
        List<BiomeSeed> seeds = CreateSampleSeeds(world, biomes, width, depth);
        RunWorldGenerators(world, seeds, biomes[0], biomes[1]);
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
