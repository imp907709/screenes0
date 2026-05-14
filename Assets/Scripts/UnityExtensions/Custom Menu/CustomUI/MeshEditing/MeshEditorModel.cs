using Meshes;
using UnityEngine;

namespace UnityExtensions.Custom_Menu.MeshEditing
{
    // editor state (pure data)
    public class MeshEditorCubeModel
    {
        public static CubeMeshBehaviour _cubeMesh = new ();
        public static GameObject _selectedObject;
        public static MeshFilter _meshFilter;
    }
}