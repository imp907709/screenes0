using System.Collections.Generic;
using UnityEngine;

public class SphereMeshGenerator : IMeshGenerator
{
    public Mesh Generate(float size)
    {
        float radius = size * 0.5f;
        const int stacks = 8;
        const int slices = 12;

        Mesh mesh = new Mesh();

        var vertices = new Vector3[(stacks + 1) * (slices + 1)];
        var triangles = new List<int>(stacks * slices * 6);

        for (int lat = 0; lat <= stacks; lat++)
        {
            float theta = Mathf.PI * lat / stacks;
            float sinT = Mathf.Sin(theta);
            float cosT = Mathf.Cos(theta);

            for (int lon = 0; lon <= slices; lon++)
            {
                float phi = 2f * Mathf.PI * lon / slices;
                float x = sinT * Mathf.Cos(phi);
                float z = sinT * Mathf.Sin(phi);
                float y = cosT;
                vertices[lat * (slices + 1) + lon] = new Vector3(x, y, z) * radius;
            }
        }

        for (int lat = 0; lat < stacks; lat++)
        {
            for (int lon = 0; lon < slices; lon++)
            {
                int a = lat * (slices + 1) + lon;
                int b = a + slices + 1;
                triangles.Add(a);
                triangles.Add(b);
                triangles.Add(a + 1);
                triangles.Add(a + 1);
                triangles.Add(b);
                triangles.Add(b + 1);
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }
}
