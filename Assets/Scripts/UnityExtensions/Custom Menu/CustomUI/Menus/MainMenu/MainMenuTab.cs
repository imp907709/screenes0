using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityExtensions.Custom_Menu.CustomUI.Menus.MeshEditing;

namespace UnityExtensions.Custom_Menu.CustomUI.Menus
{
    public class MainMenuTab : EditorWindow
    {

        private TabView _tabView;
        public void CreateGUI()
        {
            AddSubTabs();

            var mainMesh = MainMenuSubs.AddSubTab(_tabView, "Mesh Editing");
            MainTabMenus(mainMesh);
            
            var examplesBody = MainMenuSubs.AddSubTab(_tabView, "Sub Mesh");
            AddSecondaryTab(examplesBody);

            OnSelectionChange();
        }

        public void AddSubTabs()
        {
            _tabView = new TabView();
            _tabView.style.flexGrow = 1;
            rootVisualElement.Add(_tabView);
        }

        public void MainTabMenus(VisualElement root)
        {
            CustomMeshUI.CustomMeshUIAdd(root);
        }

        public void AddSecondaryTab(VisualElement root)
        {
            QubeCreatorUI.Create(root);

            VoronoiUI.AddVoronoi(root);
            VoronoiNewUI.AddVoronoiNew(root);
            FanFillMeshUI.AddFanFillMesh(root);
            SquareDropsUI.AddSquareDrops(root);
          

            BiomeUI.CustomMeshUIAdd(root);
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