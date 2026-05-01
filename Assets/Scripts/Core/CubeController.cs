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

    public void Export()
    {
        if (_current == null) return;

        string path = Application.persistentDataPath + "/cube.obj";
        _service.ExportCube(_current, path);

        Debug.Log("Exported to " + path);
    }
}