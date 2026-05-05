using System.Collections.Generic;
using UnityEngine;

#region DATA

public class Biome
{
    public int Id;
    public string Name;

    public float SpreadSpeed = 1f;
    public float Strength = 1f;
    public float Chaos = 0.5f;

    public float HeightBias = 0f;

    /// <summary>Used for cell mesh tint (vertex colors).</summary>
    public Color Color = Color.white;
}

public class Cell
{
    public Vector3 Position;

    public Biome Biome;

    public List<Cell> Neighbors = new List<Cell>();

    public bool Occupied;

    public int RegionId = -1;

    public float Height;
}

public class World
{
    public List<Cell> Cells = new List<Cell>();
}

public class BiomeSeed
{
    public Biome Biome;
    public Cell StartCell;
}

#endregion