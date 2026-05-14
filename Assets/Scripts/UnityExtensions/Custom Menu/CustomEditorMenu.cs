using UnityEditor;
using UnityEngine;

namespace UnityExtensions.Custom_Menu
{
    /// <summary>
    ///  Custom menu extension
    /// </summary>
    public class CustomEditorMenu: EditorWindow
    {
        [MenuItem("Custom Menu/Empty Menu")]
        public static void ShowDefaultWindow()
        {
            var wnd = GetWindow<CustomEditorMenu>();
            wnd.titleContent = new GUIContent("Custom menu");
        }
    }
}