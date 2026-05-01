using System.IO;
using System.Text;
using UnityEngine;

public static class ObjExporter
{
    public static void Export(Mesh mesh, Transform t, string path)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var v in mesh.vertices)
        {
            Vector3 w = t.TransformPoint(v);
            sb.AppendLine($"v {w.x} {w.y} {w.z}");
        }

        int[] tris = mesh.triangles;
        for (int i = 0; i < tris.Length; i += 3)
        {
            sb.AppendLine($"f {tris[i]+1} {tris[i+1]+1} {tris[i+2]+1}");
        }

        File.WriteAllText(path, sb.ToString());
    }
}