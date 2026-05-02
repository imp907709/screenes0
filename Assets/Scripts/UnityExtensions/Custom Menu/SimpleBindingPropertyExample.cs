using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UIToolkitExamples
{
    // Extends Unity menu (custom EditorWindow entry)
    public class SimpleBindingPropertyExample : EditorWindow
    {
        TextField m_ObjectNameBinding;

        // Read-only: selected object + component / script types (not bound to SerializedProperty)
        TextField m_selectionHeader;
        TextField m_SelectionSummary;

        
        [MenuItem("Custom Menu/Selected Name Example")]
        public static void ShowDefaultWindow()
        {
            var wnd = GetWindow<SimpleBindingPropertyExample>();
            wnd.titleContent = new GUIContent("Simple Binding Property");
        }

        // Unity hook: build UI Toolkit content for this EditorWindow
        public void CreateGUI()
        {
            m_ObjectNameBinding = new TextField("Object Name Binding");
            rootVisualElement.Add(m_ObjectNameBinding);

            m_selectionHeader = new TextField("Type & attached components");
            rootVisualElement.Add(m_selectionHeader);
   
            m_SelectionSummary = new TextField();
            m_SelectionSummary.multiline = true;
            m_SelectionSummary.isReadOnly = true;
            m_SelectionSummary.style.minHeight = 160f;
            rootVisualElement.Add(m_SelectionSummary);

            OnSelectionChange();
        }

        // Unity calls this when editor selection changes
        public void OnSelectionChange()
        {
            // GameObject selectedObject = Selection.activeObject as GameObject;
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject != null)
            {
                // Wrap the selected GameObject for serialized-property access
                SerializedObject so = new SerializedObject(selectedObject);

                // GameObject.name is stored as m_Name on disk
                SerializedProperty property = so.FindProperty("m_Name");
                // Two-way bind the TextField to that property
                m_ObjectNameBinding.BindProperty(property);

                m_SelectionSummary.SetValueWithoutNotify(BuildSelectionSummary(selectedObject));
            }
            else
            {
                // Clear binding when nothing selected
                m_ObjectNameBinding.Unbind();
                m_SelectionSummary.SetValueWithoutNotify(string.Empty);
            }
        }

        private static string BuildSelectionSummary(GameObject go)
        {
            var sb = new StringBuilder();

            // Runtime type of the GameObject instance (always UnityEngine.GameObject)
            sb.AppendLine("Object type: " + go.GetType().FullName);
            sb.AppendLine("Prefab asset type: " + (PrefabUtility.IsPartOfPrefabInstance(go) ? "Prefab instance" : "Scene object or prefab asset root"));
            sb.AppendLine();

            // Same order as Inspector: GetComponents lists all attached components
            sb.AppendLine("Attached components (Inspector order):");
            Component[] components = go.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                Component c = components[i];
                if (c == null)
                {
                    sb.AppendLine($"  [{i}] (Missing script — broken reference)");
                    continue;
                }

                System.Type t = c.GetType();
                string role = c is MonoBehaviour ? "script / behaviour" : "engine component";
                sb.AppendLine($"  [{i}] {t.FullName}  ({role})");
            }

            return sb.ToString();
        }
    }
}
