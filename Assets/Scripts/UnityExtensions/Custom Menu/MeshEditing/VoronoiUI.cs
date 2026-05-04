using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;
using Meshes.Voronoi;

namespace UnityExtensions.Custom_Menu.MeshEditing
{
    public static class VoronoiUI
    {
        private static float _size = 10f;
        private static int _siteCount = 10;
        private static int _seed = 1;

        public static void AddVoronoi(VisualElement rootVisualElement)
        {
            // SIZE
            var sizeField = new TextField("Size");
            sizeField.value = _size.ToString(CultureInfo.InvariantCulture);
            sizeField.RegisterValueChangedCallback(evt =>
            {
                float.TryParse(evt.newValue, NumberStyles.Float,
                    CultureInfo.InvariantCulture, out _size);
            });

            // SITE COUNT
            var countField = new TextField("Sites");
            countField.value = _siteCount.ToString();
            countField.RegisterValueChangedCallback(evt =>
            {
                int.TryParse(evt.newValue, out _siteCount);
            });

            // SEED
            var seedField = new TextField("Seed");
            seedField.value = _seed.ToString();
            seedField.RegisterValueChangedCallback(evt =>
            {
                int.TryParse(evt.newValue, out _seed);
            });

            // BUTTON
            var button = MenuCreation._buttonCreate("Generate Voronoi", () =>
            {
                var filter = MeshEditorCubeModel._meshFilter;

                if (filter == null || filter.sharedMesh == null)
                {
                    Debug.LogError("No mesh selected");
                    return;
                }

                Debug.Log($"Voronoi: size={_size}, sites={_siteCount}, seed={_seed}");

                // ✔ SAFE EDITOR PATTERN: clone shared mesh
                var mesh = Object.Instantiate(filter.sharedMesh);

                VoronoiApplier.Generate(
                    _siteCount,
                    _size,
                    _seed,
                    mesh
                );

                mesh.RecalculateNormals();
                mesh.RecalculateBounds();

                // assign back
                filter.sharedMesh = mesh;

                // force editor repaint
                UnityEditor.EditorUtility.SetDirty(filter);
            });

            // ADD UI
            rootVisualElement.Add(sizeField);
            rootVisualElement.Add(countField);
            rootVisualElement.Add(seedField);
            rootVisualElement.Add(button);
        }
    }
}