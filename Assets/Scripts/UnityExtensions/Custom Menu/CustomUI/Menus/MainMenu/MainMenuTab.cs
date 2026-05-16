using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityExtensions.Custom_Menu.CustomUI.Menus.MeshEditing;

namespace UnityExtensions.Custom_Menu.CustomUI.Menus
{
    public class MainMenuTab : EditorWindow
    {
        // [MenuItem("Custom Menu/Mesh Editor Menu")]
        // public static void Open()
        // {
        //     // One dock tab per EditorWindow — Unity does not support a second tab here beside "Mesh Menu".
        //     // Extra "tabs" (e.g. Sample) belong in the window body (see MainMenuTabsHostUI) or in a separate EditorWindow you dock next to this one.
        //     GetWindow<MainMenu>().titleContent = new GUIContent("Mesh Menu");
        // }

        public void CreateGUI()
        {
            rootVisualElement.Add(new Label("Mesh editing"));
            MainTabMenus();
            MainMenuSubs.WrapRootChildrenIntoDefaultTwoTabs(this);
            OnSelectionChange();
        }
        
        public void MainTabMenus()
        {
            QubeCreatorUI.Create(rootVisualElement);

            SquareDropsUI.AddSquareDrops(rootVisualElement);
            VoronoiUI.AddVoronoi(rootVisualElement);
            VoronoiNewUI.AddVoronoiNew(rootVisualElement);
            FanFillMeshUI.AddFanFillMesh(rootVisualElement);
            
            CustomMeshUI.CustomMeshUIAdd(rootVisualElement);
            
            BiomeUI.CustomMeshUIAdd(rootVisualElement);
        }

        
        public void OnSelectionChange()
        {
            // GameObject selectedObject = Selection.activeObject as GameObject;
            MeshEditorCubeModel._gameObject = Selection.activeGameObject;

            if (MeshEditorCubeModel._gameObject == null)
                return;

            MeshEditorCubeModel._meshFilter = MeshEditorCubeModel._gameObject.GetComponent<MeshFilter>();

            if (MeshEditorCubeModel._meshFilter == null)
                return;

            if (MeshEditorCubeModel._meshFilter.sharedMesh == null)
                return;

            if(MeshEditorCubeModel._mesh == null)
                return;
        
            Debug.Log($"Selected: {MeshEditorCubeModel._gameObject.name}");
            Debug.Log($"Mesh: {MeshEditorCubeModel._meshFilter.sharedMesh.name}");
        }
    }
}