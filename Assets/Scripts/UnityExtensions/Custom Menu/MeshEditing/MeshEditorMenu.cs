using UnityEditor;
using UnityEngine;

namespace UnityExtensions.Custom_Menu.MeshEditing
{
    public class MeshEditorMenu : EditorWindow
    {

        [MenuItem("Custom Menu/Mesh Editor Menu")]
        public static void Open()
        {
            GetWindow<MeshEditorMenu>().titleContent = new GUIContent("Mesh Menu");

        }

        public void CreateGUI()
        {

            var slider =  MenuCreation._sliderMenu(
                "Size",
                0.1f,
                10f,
                value =>
                {
                    if (MeshEditorCubeModel._cubeMesh == null)
                    {
                        Debug.Log("No generator");
                        return;
                    }
       
                    if (MeshEditorCubeModel._meshFilter == null)
                    {
                        Debug.Log("No mesh filter");
                        return;
                    }
       
                    Debug.Log("Mesh generator applied");
       
                    MeshEditorCubeModel._meshFilter.sharedMesh =
                        MeshEditorCubeModel._cubeMesh.Generate(value);
                }
            );
        
            rootVisualElement.Add(slider);

            
            SquareDropsUI.AddSquareDrops(rootVisualElement);
            
            VoronoiUI.AddVoronoi(rootVisualElement);
                
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