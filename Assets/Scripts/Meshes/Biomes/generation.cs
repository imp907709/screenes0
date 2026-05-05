using System.Collections.Generic;
using UnityEngine;

public static class WorldGenerator
{
    // -------------------------
    // BIOME COMPETITION
    // -------------------------
    public static void GenerateBiomes(World world, List<BiomeSeed> seeds)
    {
        Queue<Cell> q = new Queue<Cell>();

        foreach (var s in seeds)
        {
            s.StartCell.Biome = s.Biome;
            s.StartCell.Occupied = true;
            q.Enqueue(s.StartCell);
        }

        while (q.Count > 0)
        {
            var c = q.Dequeue();
            var b = c.Biome;

            foreach (var n in c.Neighbors)
            {
                float chance =
                    b.SpreadSpeed *
                    b.Strength *
                    Random.value *
                    (1f - b.Chaos * Random.value);

                if (chance < 0.5f)
                    continue;

                if (!n.Occupied)
                {
                    n.Biome = b;
                    n.Occupied = true;
                    q.Enqueue(n);
                }
            }
        }
    }

    // -------------------------
    // REGION LABELING
    // -------------------------
    public static void GenerateRegions(World world)
    {
        int regionId = 0;
        HashSet<Cell> visited = new HashSet<Cell>();

        foreach (var c in world.Cells)
        {
            if (visited.Contains(c) || c.Biome == null)
                continue;

            Queue<Cell> q = new Queue<Cell>();
            q.Enqueue(c);

            c.RegionId = regionId;
            visited.Add(c);

            while (q.Count > 0)
            {
                var current = q.Dequeue();

                foreach (var n in current.Neighbors)
                {
                    if (visited.Contains(n)) continue;
                    if (n.Biome != c.Biome) continue;

                    n.RegionId = regionId;
                    visited.Add(n);
                    q.Enqueue(n);
                }
            }

            regionId++;
        }
    }

    // -------------------------
    // HEIGHT SHAPING
    // -------------------------
    public static void GenerateHeight(World world)
    {
        Dictionary<int, int> regionSize = new Dictionary<int, int>();

        foreach (var c in world.Cells)
        {
            if (!regionSize.ContainsKey(c.RegionId))
                regionSize[c.RegionId] = 0;

            regionSize[c.RegionId]++;
        }

        foreach (var c in world.Cells)
        {
            float h = c.Biome != null ? c.Biome.HeightBias : 0f;

            float regionFactor = regionSize[c.RegionId] * 0.01f;
            float noise = Random.Range(-0.15f, 0.15f);

            c.Height = h + regionFactor + noise;
        }
    }

    public static void Generate(World world, List<BiomeSeed> seeds)
    {
        GenerateBiomes(world, seeds);
        GenerateRegions(world);
        GenerateHeight(world);
    }
}