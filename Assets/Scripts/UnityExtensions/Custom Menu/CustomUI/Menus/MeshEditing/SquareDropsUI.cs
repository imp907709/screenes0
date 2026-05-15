using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;
using UnityExtensions.Custom_Menu.Core;

namespace UnityExtensions.Custom_Menu.CustomUI.MeshEditing
{
    public static class SquareDropsUI
    {
        private static float _size = 10f;
        private static int _count = 10;
        private static int _seed = 1;

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
            seedField.value = _seed.ToString();
            seedField.RegisterValueChangedCallback(evt =>
            {
                int.TryParse(evt.newValue, out _seed);
            });

            // BUTTON
            var button = MenuCreation._buttonCreate("Create Square Drops", () =>
            {
                var filter = MeshEditorCubeModel._meshFilter;

                if (filter == null || filter.sharedMesh == null)
                {
                    Debug.LogError("No mesh selected");
                    return;
                }

                Debug.Log($"Square drops: size={_size}, count={_count}, seed={_seed}");

                try
                {
                    var mesh = Object.Instantiate(filter.sharedMesh);

                    SquareDropsMeshApplier.GenerateAndApply(mesh, _size, _count, _seed);

                    if (mesh.vertexCount == 0)
                    {
                        Debug.LogError("Square drops produced no vertices.");
                        return;
                    }

                    filter.sharedMesh = mesh;
                    UnityEditor.EditorUtility.SetDirty(filter);
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
            });

            // ADD UI
            rootVisualElement.Add(sizeField);
            rootVisualElement.Add(countField);
            rootVisualElement.Add(seedField);
            rootVisualElement.Add(button);
        }
    }
}