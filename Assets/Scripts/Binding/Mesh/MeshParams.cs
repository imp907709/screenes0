using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GroundMeshApplier : MonoBehaviour
{
    [Header("Params")]
    public float SizeX = 10f;

    private GroundMeshGenerator _generator;
    private MeshFilter _meshFilter;

    private void EnsureInit()
    {
        if (_meshFilter == null)
            _meshFilter = GetComponent<MeshFilter>();

        if (_generator == null)
            _generator = new GroundMeshGenerator();
    }
    
    // one on load to scene
    void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _generator = new GroundMeshGenerator();
        Debug.Log($"Awake");
        Debug.Log($"Mesh generator inited = {_generator}");
    }

    // on enable
    
    // 
    void Start()
    {
        Debug.Log($"Start");
        Apply();
    }   

    // on value changed
    private void OnValidate()
    {
        EnsureInit();
        
        Debug.Log($"OnValidate");
        // avoid running before Awake in editor
        if (!Application.isPlaying && _meshFilter == null)
            _meshFilter = GetComponent<MeshFilter>();

        Apply();
    }

    public void Apply()
    {
        Debug.Log($"Apply");
        var p = GroundMeshGeneratorParams.Default;
        p.sizeX = SizeX;
        p.sizeZ = SizeX;

        var mesh = _generator.Generate(p);

        _meshFilter.sharedMesh = mesh;

        Debug.Log($"Mesh regenerated with SizeX={SizeX}");
    }
}