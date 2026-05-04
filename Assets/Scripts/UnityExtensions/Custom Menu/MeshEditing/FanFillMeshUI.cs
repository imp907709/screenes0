using System.Globalization;
using Meshes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityExtensions.Custom_Menu.MeshEditing
{
    /// <summary>
    /// Creates a new scene object with a fan-filled patch (5 or 7 rim points). No selection required.
    /// </summary>
    public static class FanFillMeshUI
    {
        private static float _scale = 2.5f;
        private static int _rimVertexCount = 7;

        public static void AddFanFillMesh(VisualElement rootVisualElement)
        {
            var scaleField = new TextField("Cell scale");
            scaleField.value = _scale.ToString(CultureInfo.InvariantCulture);
            scaleField.RegisterValueChangedCallback(evt =>
            {
                float.TryParse(evt.newValue, NumberStyles.Float, CultureInfo.InvariantCulture, out _scale);
            });

            var countField = new TextField("Rim points (5 or 7)");
            countField.value = _rimVertexCount.ToString();
            countField.RegisterValueChangedCallback(evt =>
            {
                if (int.TryParse(evt.newValue, out int n) && (n == 5 || n == 7))
                    _rimVertexCount = n;
            });

            var button = MenuCreation._buttonCreate("Generate mesh", () =>
            {
                try
                {
                    var rim = SimpleFanFillMesh.BuildIrregularCellRim(_rimVertexCount, _scale);
                    Vector3 hub = SimpleFanFillMesh.Centroid(rim);
                    Mesh mesh = SimpleFanFillMesh.Create(hub, rim);

                    var go = new GameObject("FanFillPatch");
                    Undo.RegisterCreatedObjectUndo(go, "Fan fill patch");
                    var mf = go.AddComponent<MeshFilter>();
                    var mr = go.AddComponent<MeshRenderer>();
                    mf.sharedMesh = mesh;

                    Shader shader = Shader.Find("Universal Render Pipeline/Lit")
                        ?? Shader.Find("HDRP/Lit")
                        ?? Shader.Find("Standard")
                        ?? Shader.Find("Unlit/Color");
                    if (shader != null)
                        mr.sharedMaterial = new Material(shader);

                    if (Selection.activeTransform != null)
                        go.transform.SetPositionAndRotation(Selection.activeTransform.position, Selection.activeTransform.rotation);
                    else
                        go.transform.position = Vector3.zero;

                    Selection.activeGameObject = go;
                    Debug.Log($"Fan fill patch: rim={rim.Length}, verts={mesh.vertexCount}, tris={mesh.triangles.Length / 3}");
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
            });

            rootVisualElement.Add(scaleField);
            rootVisualElement.Add(countField);
            rootVisualElement.Add(button);
        }

      
    }
}
