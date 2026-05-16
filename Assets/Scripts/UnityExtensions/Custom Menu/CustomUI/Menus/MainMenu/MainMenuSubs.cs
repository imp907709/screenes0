using UnityEngine.UIElements;

namespace UnityExtensions.Custom_Menu.CustomUI.Menus
{
    public static class MainMenuSubs
    {
        public static void WrapRootChildrenIntoDefaultTwoTabs(MainMenuTab window)
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
