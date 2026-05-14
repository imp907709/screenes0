using UnityEditor;
using UnityEngine;
using UnityExtensions.Custom_Menu.MeshEditing.CustomUI;

namespace UnityExtensions.Custom_Menu.MeshEditing
{
    public class MainMenu : EditorWindow
    {

        [MenuItem("Custom Menu/Mesh Editor Menu")]
        public static void Open()
        {
            GetWindow<MainMenu>().titleContent = new GUIContent("Mesh Menu");

        }

        public void CreateGUI()
        {

            MenuCreation.Create(rootVisualElement);

            SquareDropsUI.AddSquareDrops(rootVisualElement);
            VoronoiUI.AddVoronoi(rootVisualElement);
            VoronoiNewUI.AddVoronoiNew(rootVisualElement);
            FanFillMeshUI.AddFanFillMesh(rootVisualElement);
            
            CustomMeshUI.CustomMeshUIAdd(rootVisualElement);
            
            BiomeUI.CustomMeshUIAdd(rootVisualElement);

            OnSelectionChange();
        }

        
        public void OnSelectionChange()
        {
            // GameObject selectedObject = Selection.activeObject as GameObject;
            MeshEditorCubeModel._selectedObject = Selection.activeGameObject;

            if (MeshEditorCubeModel._selectedObject == null)
                return;

            MeshEditorCubeModel._meshFilter = MeshEditorCubeModel._selectedObject.GetComponent<MeshFilter>();

            if (MeshEditorCubeModel._meshFilter == null)
                return;

            if (MeshEditorCubeModel._meshFilter.sharedMesh == null)
                return;

            if(MeshEditorCubeModel._cubeMesh == null)
                return;
        
            Debug.Log($"Selected: {MeshEditorCubeModel._selectedObject.name}");
            Debug.Log($"Mesh: {MeshEditorCubeModel._meshFilter.sharedMesh.name}");
        }
    }
}