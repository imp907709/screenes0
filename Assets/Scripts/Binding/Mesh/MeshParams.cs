using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GroundMeshApplier : MonoBehaviour
{
    [Header("Params")]
    public float SizeX = 10f;
    [FormerlySerializedAs("SizeY")] public float SizeZ = 10f;
    public float HeightMin = -0.2f;
    public float HeightMax = 0.9f;
    public int SegmentsX = 48;
    public int SegmentsZ = 48;
    public float NoiseScale = 0.11f;
    public int NoiseOctaves = 4;
    public float NoisePersistence = 0.48f;
    public float NoiseLacunarity = 2.05f;
    public Vector3 NoiseOffset;
    public uint NoiseSeed = 42u;

    private GroundMeshGenerator _generator;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    private Material _mat;
    
    private void EnsureInit()
    {
        if (_meshFilter == null)
            _meshFilter = GetComponent<MeshFilter>();
        
        if (_meshRenderer == null)
            _meshRenderer = GetComponent<MeshRenderer>();
        
        if (_generator == null)
            _generator = new GroundMeshGenerator();

        if (_mat == null)
        {
            var shaderName = "Universal Render Pipeline/Lit";
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if(shader == null)
                Debug.Log($"SHADER IS NULL FOR {shaderName}");
            
            // _mat =  new Material(Shader.Find("Standard"));
            // _mat =  new Material(Shader.Find(" Universal Render Pipeline/Lit"));
            _mat = GraphicsSettings.defaultRenderPipeline.defaultMaterial;
            if(_mat == null)
                Debug.Log($"MATERIAL IS NULL FOR {GraphicsSettings.defaultRenderPipeline.defaultMaterial}");
        }

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
        p.sizeZ = SizeZ;
        p.heightMin = HeightMin;
        p.heightMax = HeightMax;
        p.segmentsX = SegmentsX;
        p.segmentsZ = SegmentsZ;
        p.noiseScale = NoiseScale;
        p.noiseOctaves = NoiseOctaves;
        p.noisePersistence = NoisePersistence;
        p.noiseLacunarity = NoiseLacunarity;
        p.noiseOffset = NoiseOffset;
        p.noiseSeed = NoiseSeed;

        var mesh = _generator.Generate(p);
        mesh.name = "GroundMesh";

        _meshFilter.sharedMesh = mesh;
        Debug.Log($"Material ={_mat}");
        _meshRenderer.sharedMaterial = _mat;
        
        Debug.Log($"Mesh regenerated with SizeX={SizeX}");
        
#if UNITY_EDITOR
        UnityEditor.SceneView.RepaintAll();
#endif
    }
}
