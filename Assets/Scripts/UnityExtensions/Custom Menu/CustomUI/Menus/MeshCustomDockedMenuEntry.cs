using UnityEditor;
using UnityEngine;
using UnityExtensions.Custom_Menu;

namespace UnityExtensions.Custom_Menu.CustomUI
{
    /// <summary>Opens <see cref="MainMenu"/> and <see cref="CustomEditorMenu"/> as sibling dock tabs (does not change existing single-window menu items).</summary>
    public static class MeshCustomDockedMenuEntry
    {
        [MenuItem("Custom Menu/Mesh + Custom (docked)")]
        public static void OpenMeshAndCustomDocked()
        {
            var mesh = EditorWindow.GetWindow<MainMenu>();
            mesh.titleContent = new GUIContent("Mesh Menu");

            var custom = EditorWindow.GetWindow<CustomEditorMenu>(typeof(MainMenu));
            custom.titleContent = new GUIContent("Empty tab menu");
        }
    }
}
