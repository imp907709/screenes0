using UnityEngine;
using System;

public class CubeController : MonoBehaviour
{
    public float size = 1f;

    public event Action<GameObject> OnCubeCreated;

    private CubeService _service = new CubeService();
    private GameObject _current;

    public void Generate()
    {
        _current = _service.CreateCube(size);
        _current.transform.position = Vector3.zero;

        OnCubeCreated?.Invoke(_current);
    }

    public void ReGenerate()
    {
        if (_current != null)
        {
            Destroy(_current);
            _current = null;
        }

        Generate();
    }

    public void Export()
    {
        if (_current == null) return;

        string path = Application.persistentDataPath + "/cube.obj";
        _service.ExportCube(_current, path);

        Debug.Log("Exported to " + path);
    }

    /// <summary>
    /// Editor: saves the active cube mesh as a project asset (e.g. under Assets/GeneratedMeshes/).
    /// In builds this logs a warning; use <see cref="Export"/> for a loose .obj file instead.
    /// </summary>
    public void ExportMeshAsProjectAsset(string assetPath = "Assets/GeneratedMeshes/GeneratedCube.asset")
    {
        if (_current == null) return;

        var mf = _current.GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null) return;

        CubeMeshProjectExporter.SaveMeshAsAsset(mf.sharedMesh, assetPath);
    }
}