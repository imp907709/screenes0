using UnityEngine;

/// <summary>Maps a regular XZ vertex grid (row-major in Z) to a 2D height array [x,z].</summary>
public static class TerrainHeightmapGrid
{
    public static bool VertexCountMatches(int vertexCount, int gridX, int gridZ) =>
        vertexCount == gridX * gridZ;

    public static float[,] FromVertices(Vector3[] verts, int gridX, int gridZ)
    {
        var map = new float[gridX, gridZ];
        for (int z = 0; z < gridZ; z++)
        {
            for (int x = 0; x < gridX; x++)
                map[x, z] = verts[z * gridX + x].y;
        }

        return map;
    }

    public static void WriteYToVertices(Vector3[] verts, float[,] heights, int gridX, int gridZ)
    {
        for (int z = 0; z < gridZ; z++)
        {
            for (int x = 0; x < gridX; x++)
                verts[z * gridX + x].y = heights[x, z];
        }
    }
}
