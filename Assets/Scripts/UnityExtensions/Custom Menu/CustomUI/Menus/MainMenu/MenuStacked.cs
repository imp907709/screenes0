using UnityEditor;
using UnityEngine;
using UnityExtensions.Custom_Menu.CustomUI.Menus.Examples;

// Tabbed mainmenu
namespace UnityExtensions.Custom_Menu.CustomUI.Menus
{
    /// <summary>Opens <see cref="MainMenuTab"/> and <see cref="TemplateEmptyMenu"/> as sibling dock tabs (does not change existing single-window menu items).</summary>
    public static class MenuStacked
    {
        
        // global menu name
        [MenuItem("Custom Menu/Mesh Stacked Menu")]
        public static void OpenMeshAndCustomDocked()
        {
            // 1 lvlv tab 1
            var mesh = EditorWindow.GetWindow<MainMenuTab>();
            mesh.titleContent = new GUIContent("Mesh tab");

            // 1 lvlv tab 2
            var custom = EditorWindow.GetWindow<TemplateEmptyMenu>(typeof(MainMenuTab));
            custom.titleContent = new GUIContent("Sample tab");
        }
    }
}
