using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    public CubeController controller;

    private GenerateButtonBinder _binder;

    void Awake()
    {
        if (controller == null)
            controller = FindAnyObjectByType<CubeController>();
        if (controller == null)
            controller = gameObject.AddComponent<CubeController>();

        _binder = GetComponent<GenerateButtonBinder>();
        if (_binder == null)
            _binder = gameObject.AddComponent<GenerateButtonBinder>();
        _binder.Init(controller);
    }
}