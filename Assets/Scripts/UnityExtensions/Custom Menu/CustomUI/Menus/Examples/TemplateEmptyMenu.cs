using UnityEditor;
using UnityEngine;

namespace UnityExtensions.Custom_Menu.CustomUI.Menus.Examples
{
    /// <summary>
    ///  Custom menu extension
    /// </summary>
    public class TemplateEmptyMenu: EditorWindow
    {
        [MenuItem("Custom Menu/Empty Menu")]
        public static void ShowDefaultWindow()
        {
            var wnd = GetWindow<TemplateEmptyMenu>();
            wnd.titleContent = new GUIContent("Custom menu");
        }
    }
}