using UnityEngine;

public class CubeMeshGenerator
{
    public Mesh Generate(float size)
    {
        float h = size * 0.5f;

        Mesh mesh = new Mesh();

        Vector3[] vertices =
        {
            new Vector3(-h,-h,-h),
            new Vector3(h,-h,-h),
            new Vector3(h,h,-h),
            new Vector3(-h,h,-h),

            new Vector3(-h,-h,h),
            new Vector3(h,-h,h),
            new Vector3(h,h,h),
            new Vector3(-h,h,h),
        };

        int[] triangles =
        {
            0,2,1, 0,3,2,
            1,2,6, 6,5,1,
            5,6,7, 7,4,5,
            4,7,3, 3,0,4,
            3,7,6, 6,2,3,
            4,0,1, 1,5,4
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
}