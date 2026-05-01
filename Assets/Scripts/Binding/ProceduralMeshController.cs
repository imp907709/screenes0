using System;
using Init;
using UnityEngine;

/// <summary>
/// Single controller for procedural meshes; pick shape via SerializeReference on <see cref="_shapeBinding"/>.
/// </summary>
public class ProceduralMeshController : MonoBehaviour, IProceduralMeshController
{
    [SerializeReference]
    [SerializeField]
    private MeshShapeBinding _shapeBinding;

    public float size = 1f;

    public event Action<GameObject> OnMeshObjectCreated;

    private ProceduralMeshService _service;
    private GameObject _current;

    private void Awake()
    {
        if (_shapeBinding == null)
            _shapeBinding = new CubeMeshShapeBinding();

        RebuildService();
    }

    private void RebuildService()
    {
        _service = new ProceduralMeshService(_shapeBinding.CreateGenerator(), _shapeBinding.RootObjectName);
    }

    /// <summary>
    /// Replaces the active shape using a registry id (e.g. <c>cube</c>, <c>sphere</c>).
    /// Unknown ids fall back to the first registry entry.
    /// </summary>
    public void SetShapeById(string shapeId)
    {
        if (!MeshShapeRegistry.TryGetById(shapeId, out var option))
        {
            if (!MeshShapeRegistry.TryGetById(MeshShapeIds.Default, out option))
                option = MeshShapeRegistry.All[0];
        }

        _shapeBinding = option.CreateBinding();
        RebuildService();
    }

    public void Generate()
    {
        _current = _service.Create(size);
        _current.transform.position = Vector3.zero;

        OnMeshObjectCreated?.Invoke(_current);
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

        string path = Application.persistentDataPath + "/" + _shapeBinding.ObjFileBaseName + ".obj";
        _service.ExportObj(_current, path);

        Debug.Log("Exported to " + path);
    }

    public void ExportMeshAsProjectAsset(string assetPath = null)
    {
        if (_current == null) return;

        var mf = _current.GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null) return;

        if (string.IsNullOrEmpty(assetPath))
            assetPath = _shapeBinding.DefaultProjectAssetPath;

        MeshProjectExporter.SaveMeshAsAsset(mf.sharedMesh, assetPath);
    }
}
