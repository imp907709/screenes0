using UnityEditor;
using UnityEngine;
using UnityExtensions.Custom_Menu.CustomUI.Menus.Examples;

// Tabbed mainmenu
namespace UnityExtensions.Custom_Menu.CustomUI.Menus
{
    /// <summary>Opens <see cref="MainMenuTab"/> and <see cref="TemplateEmptyMenu"/> as sibling dock tabs (does not change existing single-window menu items).</summary>
    public static class MenuStacked
    {
        [MenuItem("Stacked Menu")]
        public static void OpenMeshAndCustomDocked()
        {
            var mesh = EditorWindow.GetWindow<MainMenuTab>();
            mesh.titleContent = new GUIContent("Mesh tab");

            var custom = EditorWindow.GetWindow<TemplateEmptyMenu>(typeof(MainMenuTab));
            custom.titleContent = new GUIContent("Sample tab");
        }
    }
}
