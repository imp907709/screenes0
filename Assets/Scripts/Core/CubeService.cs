using UnityEngine;

public class CubeService
{
    private readonly CubeMeshGenerator _generator = new CubeMeshGenerator();

    public GameObject CreateCube(float size)
    {
        Mesh mesh = _generator.Generate(size);

        GameObject go = new GameObject("GeneratedCube");
        go.AddComponent<MeshFilter>().mesh = mesh;
        go.AddComponent<MeshRenderer>();

        return go;
    }

    public void ExportCube(GameObject go, string path)
    {
        var mf = go.GetComponent<MeshFilter>();
        if (mf == null) return;

        ObjExporter.Export(mf.mesh, go.transform, path);
    }
}