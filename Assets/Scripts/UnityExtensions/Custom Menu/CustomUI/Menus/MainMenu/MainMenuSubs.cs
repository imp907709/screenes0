using UnityEngine.UIElements;

namespace UnityExtensions.Custom_Menu.CustomUI.Menus
{
    public static class MainMenuSubs
    {
        public static VisualElement AddSubTab(TabView tabView, string tabLabel)
        {
            var examplesBody = new VisualElement();
            var sampleTab = new Tab(tabLabel);
            sampleTab.Add(examplesBody);
            tabView.Add(sampleTab);
            return examplesBody;
        }
        
        public static void AddMeshSubTab(TabView tabView, VisualElement meshBody, string tabLabel = "Mesh Sub")
        {
            var meshTab = new Tab(tabLabel);
            meshTab.Add(meshBody);
            tabView.Add(meshTab);
        }

        /// <summary>Creates an empty Examples tab on <paramref name="tabView"/>; returns the body so callers can fill it (e.g. <c>SquareDropsUI.AddSquareDrops(body)</c>).</summary>
        public static VisualElement AddExamplesSubTab(TabView tabView, string tabLabel = "Examples Sub")
        {
            var examplesBody = new VisualElement();
            var sampleTab = new Tab(tabLabel);
            sampleTab.Add(examplesBody);
            tabView.Add(sampleTab);
            return examplesBody;
        }

        public static void WrapRootChildrenIntoDefaultTwoTabsOld(MainMenuTab window)
        {
            var root = window.rootVisualElement;
            var meshBody = new VisualElement();
            while (root.childCount > 0)
            {
                var c = root[0];
                c.RemoveFromHierarchy();
                meshBody.Add(c);
            }

            var tabView = new TabView();
            tabView.style.flexGrow = 1;

            var meshTab = new Tab("Mesh Sub");
            meshTab.Add(meshBody);
            tabView.Add(meshTab);

            var sampleTab = new Tab("Examples Sub");
            sampleTab.Add(new Label());
            tabView.Add(sampleTab);

            root.Add(tabView);
        }
    }
}
