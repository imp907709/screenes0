using Meshes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MeshEditorMenu : EditorWindow
{
    private CubeMeshBehaviour _cubeMesh = new ();
    private Mesh _mesh;
    private GameObject _selectedObject;
    private MeshFilter _meshFilter;

    [MenuItem("Custom Menu/Mesh Editor Menu")]
    public static void Open()
    {
        GetWindow<MeshEditorMenu>().titleContent = new GUIContent("Mesh Menu");

    }

    public void CreateGUI()
    {
        var slider = new Slider("Size", 0.1f, 10f);
        slider.value = 1f;

        slider.RegisterValueChangedCallback(evt => {
            if (_cubeMesh == null )
            {
                Debug.Log("No generator");
                return;
            }
            if (_cubeMesh._mesh == null)
            {
                Debug.Log("No mesh");
                return;
            }

            Debug.Log("Mesh generator applyed");
            
            _cubeMesh.Generate(evt.newValue);
            _meshFilter.sharedMesh = _cubeMesh._mesh;
        });

        rootVisualElement.Add(slider);

        OnSelectionChange();
    }

    public void OnSelectionChange()
    {
        // GameObject selectedObject = Selection.activeObject as GameObject;
        _selectedObject = Selection.activeGameObject;

        if (_selectedObject == null)
            return;

        _meshFilter = _selectedObject.GetComponent<MeshFilter>();

        if (_meshFilter == null)
            return;

        if (_meshFilter.sharedMesh == null)
            return;

        _mesh = _meshFilter.sharedMesh;
        
        if(_cubeMesh == null)
            return;
        
        _cubeMesh._mesh = _mesh;
        Debug.Log($"Selected: {_selectedObject.name}");
        Debug.Log($"Mesh: {_meshFilter.sharedMesh.name}");
    }
}