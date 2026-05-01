using Init;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField]
    private string defaultShapeId = MeshShapeIds.Default;

    public ProceduralMeshController controller;

    private GenerateButtonBinder _binder;
    private GeometrySelectorBinder _geometryBinder;

    void Awake()
    {
        if (controller == null)
            controller = FindAnyObjectByType<ProceduralMeshController>();
        if (controller == null)
            controller = gameObject.AddComponent<ProceduralMeshController>();

        controller.SetShapeById(defaultShapeId);

        _binder = GetComponent<GenerateButtonBinder>();
        if (_binder == null)
            _binder = gameObject.AddComponent<GenerateButtonBinder>();
        _binder.Init(controller);

        _geometryBinder = GetComponent<GeometrySelectorBinder>();
        if (_geometryBinder == null)
            _geometryBinder = gameObject.AddComponent<GeometrySelectorBinder>();
        _geometryBinder.Init(controller, defaultShapeId);
    }
}