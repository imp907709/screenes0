using UnityEngine.UIElements;

namespace UnityExtensions.Custom_Menu.CustomUI
{
    public static class MainMenuTabsHostUI
    {
        public static void WrapRootChildrenIntoDefaultTwoTabs(MainMenu window)
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
            var meshTab = new Tab("Mesh Menu");
            meshTab.Add(meshBody);
            tabView.Add(meshTab);
            
            var sampleTab = new Tab("Sample");
            sampleTab.Add(new Label());
            tabView.Add(sampleTab);
            root.Add(tabView);
        }
    }
}
