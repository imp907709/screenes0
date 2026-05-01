using UnityEngine;

public class ProceduralMeshService
{
    private readonly IMeshGenerator _generator;
    private readonly string _gameObjectName;

    public ProceduralMeshService(IMeshGenerator generator, string gameObjectName)
    {
        _generator = generator;
        _gameObjectName = gameObjectName;
    }

    public GameObject Create(float size)
    {
        Mesh mesh = _generator.Generate(size);

        GameObject go = new GameObject(_gameObjectName);
        go.AddComponent<MeshFilter>().mesh = mesh;
        go.AddComponent<MeshRenderer>();

        return go;
    }

    public void ExportObj(GameObject go, string path)
    {
        var mf = go.GetComponent<MeshFilter>();
        if (mf == null) return;

        MeshExporter.Export(mf.mesh, go.transform, path);
    }
}
