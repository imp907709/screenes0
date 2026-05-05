using System.Globalization;
using Meshes.Voronoi.FortunesVoronoi;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityExtensions.Custom_Menu.MeshEditing
{
    public static class VoronoiNewUI
    {
        private static float _size = 10f;
        private static int _siteCount = 24;
        private static int _seed = 1;
        private static float _lineY = 0.01f;

        public static void AddVoronoiNew(VisualElement rootVisualElement)
        {
            var sizeField = new TextField("Fortune Size");
            sizeField.value = _size.ToString(CultureInfo.InvariantCulture);
            sizeField.RegisterValueChangedCallback(evt =>
            {
                if (float.TryParse(evt.newValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed))
                    _size = Mathf.Max(0.01f, parsed);
            });

            var countField = new TextField("Fortune Sites");
            countField.value = _siteCount.ToString();
            countField.RegisterValueChangedCallback(evt =>
            {
                if (int.TryParse(evt.newValue, out int parsed))
                    _siteCount = Mathf.Max(3, parsed);
            });

            var seedField = new TextField("Fortune Seed");
            seedField.value = _seed.ToString();
            seedField.RegisterValueChangedCallback(evt =>
            {
                if (int.TryParse(evt.newValue, out int parsed))
                    _seed = parsed;
            });

            var lineYField = new TextField("Fortune Line Y");
            lineYField.value = _lineY.ToString(CultureInfo.InvariantCulture);
            lineYField.RegisterValueChangedCallback(evt =>
            {
                if (float.TryParse(evt.newValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed))
                    _lineY = parsed;
            });

            var button = MenuCreation._buttonCreate("Generate Voronoi (Fortune)", () =>
            {
                try
                {
                    var go = new GameObject("VoronoiFortuneNew");
                    Undo.RegisterCreatedObjectUndo(go, "Create Voronoi Fortune");

                    var demo = go.AddComponent<VoronoiDemoNew>();
                    demo.siteCount = Mathf.Max(3, _siteCount);
                    demo.size = Mathf.Max(0.01f, _size);
                    demo.seed = _seed;
                    demo.lineY = _lineY;
                    demo.generateOnStart = false;
                    demo.regenerateOnValidate = false;
                    demo.Generate();

                    if (Selection.activeTransform != null)
                        go.transform.SetPositionAndRotation(Selection.activeTransform.position, Selection.activeTransform.rotation);
                    else
                        go.transform.position = Vector3.zero;

                    Selection.activeGameObject = go;
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
            });

            rootVisualElement.Add(sizeField);
            rootVisualElement.Add(countField);
            rootVisualElement.Add(seedField);
            rootVisualElement.Add(lineYField);
            rootVisualElement.Add(button);
        }
    }
}
