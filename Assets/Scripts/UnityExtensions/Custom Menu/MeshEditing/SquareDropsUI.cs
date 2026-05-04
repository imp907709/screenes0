using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityExtensions.Custom_Menu.MeshEditing
{
    public static class SquareDropsUI
    {
        private static float _size = 10f;
        private static int _count = 10;
        private static float _seed = 1f;

        public static void AddSquareDrops(VisualElement rootVisualElement)
        {
            // SIZE
            var sizeField = new TextField("Size");
            sizeField.value = _size.ToString(CultureInfo.InvariantCulture);
            sizeField.RegisterValueChangedCallback(evt =>
            {
                float.TryParse(evt.newValue, NumberStyles.Float,
                    CultureInfo.InvariantCulture, out _size);
            });

            // COUNT
            var countField = new TextField("Count");
            countField.value = _count.ToString();
            countField.RegisterValueChangedCallback(evt =>
            {
                int.TryParse(evt.newValue, out _count);
            });

            // SEED
            var seedField = new TextField("Seed");
            seedField.value = _seed.ToString(CultureInfo.InvariantCulture);
            seedField.RegisterValueChangedCallback(evt =>
            {
                float.TryParse(evt.newValue, NumberStyles.Float,
                    CultureInfo.InvariantCulture, out _seed);
            });

            // BUTTON
            var button = MenuCreation._buttonCreate("Create Voronoi", () =>
            {
                Debug.Log($"Generate Voronoi: size={_size}, count={_count}, seed={_seed}");

                SquareDropsMeshApplier.GenerateAndApply(
                    MeshEditorCubeModel._meshFilter.sharedMesh,
                    _size,
                    _count
                );
            });

            // ADD UI
            rootVisualElement.Add(sizeField);
            rootVisualElement.Add(countField);
            rootVisualElement.Add(seedField);
            rootVisualElement.Add(button);
        }
    }
}