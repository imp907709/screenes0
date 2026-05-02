using UnityEditor;
using UnityEngine;

namespace UIToolkitExamples
{
    /// <summary>
    ///  Custom menu extension
    /// </summary>
    public class CustomEditorMenu: EditorWindow
    {
        [MenuItem("Custom Menu/Editor Menu")]
        public static void ShowDefaultWindow()
        {
            var wnd = GetWindow<CustomEditorMenu>();
            wnd.titleContent = new GUIContent("Custom menu");
        }
    }
}