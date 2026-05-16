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
            InitSubTabs();

            var mainMesh = AddSubTab(_tabView, "Mesh Editing");
            MainTabMenus(mainMesh);
            
            var examplesBody = AddSubTab(_tabView, "Sub Mesh");
            AddSecondaryTab(examplesBody);

            OnSelectionChange();
        }

        public void InitSubTabs()
        {
            _tabView = new TabView();
            _tabView.style.flexGrow = 1;
            rootVisualElement.Add(_tabView);
        }

        
        public static VisualElement AddSubTab(TabView tabView, string tabLabel)
        {
            var examplesBody = new VisualElement();
            var sampleTab = new Tab(tabLabel);
            sampleTab.Add(examplesBody);
            tabView.Add(sampleTab);
            return examplesBody;
        }
        
        public void MainTabMenus(VisualElement root)
        {
            ManualUI.Init(root);
        }

        public void AddSecondaryTab(VisualElement root)
        {
            QubeCreatorUI.Create(root);

            VoronoiUI.AddVoronoi(root);
            VoronoiNewUI.AddVoronoiNew(root);
            FanFillMeshUI.AddFanFillMesh(root);
            SquareDropsUI.AddSquareDrops(root);
            
            CustomMeshUI.CustomMeshUIAdd(root);

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