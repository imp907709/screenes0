using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>Builds a flat XZ grid mesh: <paramref name="vertsX"/>×<paramref name="vertsZ"/> vertices; world size is (vertsX−1)×(vertsZ−1) (one unit per quad). Row-major: index = z * vertsX + x.</summary>
public static class TerrainAdvancedPlaneMeshFactory
{
    public static Mesh CreateXZPlane(int vertsX, int vertsZ)
    {
        vertsX = Mathf.Max(2, vertsX);
        vertsZ = Mathf.Max(2, vertsZ);
        int sx = vertsX - 1;
        int sz = vertsZ - 1;

        float sizeX = Mathf.Max(0.001f, vertsX - 1f);
        float sizeZ = Mathf.Max(0.001f, vertsZ - 1f);
        float halfX = sizeX * 0.5f;
        float halfZ = sizeZ * 0.5f;

        int vx = vertsX;
        int vz = vertsZ;
        var vertices = new Vector3[vx * vz];
        var uvs = new Vector2[vx * vz];

        for (int z = 0; z < vz; z++)
        {
            float tz = vz > 1 ? z / (float)(vz - 1) : 0f;
            float worldZ = Mathf.Lerp(-halfZ, halfZ, tz);
            for (int x = 0; x < vx; x++)
            {
                float tx = vx > 1 ? x / (float)(vx - 1) : 0f;
                float worldX = Mathf.Lerp(-halfX, halfX, tx);
                int i = z * vx + x;
                vertices[i] = new Vector3(worldX, 0f, worldZ);
                uvs[i] = new Vector2(tx, tz);
            }
        }

        var triangles = new List<int>(sx * sz * 6);
        for (int z = 0; z < sz; z++)
        {
            for (int x = 0; x < sx; x++)
            {
                int i0 = z * vx + x;
                int i1 = i0 + 1;
                int i2 = i0 + vx;
                int i3 = i2 + 1;

                triangles.Add(i0);
                triangles.Add(i2);
                triangles.Add(i1);
                triangles.Add(i1);
                triangles.Add(i2);
                triangles.Add(i3);
            }
        }

        var mesh = new Mesh { name = "TerrainAdvancedPlane" };
        if (vertices.Length > 65535)
            mesh.indexFormat = IndexFormat.UInt32;

        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
}
