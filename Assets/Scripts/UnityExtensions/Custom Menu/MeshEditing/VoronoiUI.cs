using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;
using Meshes.Voronoi.VoronatorUsage;

namespace UnityExtensions.Custom_Menu.MeshEditing
{
    public static class VoronoiUI
    {
        private static float _size = 10f;
        private static int _siteCount = 10;
        private static int _seed = 1;

        public static void AddVoronoi(VisualElement rootVisualElement)
        {
            var sizeField = new TextField("Size");
            sizeField.value = _size.ToString(CultureInfo.InvariantCulture);
            sizeField.RegisterValueChangedCallback(evt =>
            {
                float.TryParse(evt.newValue, NumberStyles.Float,
                    CultureInfo.InvariantCulture, out _size);
            });

            var countField = new TextField("Sites");
            countField.value = _siteCount.ToString();
            countField.RegisterValueChangedCallback(evt =>
            {
                int.TryParse(evt.newValue, out _siteCount);
            });

            var seedField = new TextField("Seed");
            seedField.value = _seed.ToString();
            seedField.RegisterValueChangedCallback(evt =>
            {
                int.TryParse(evt.newValue, out _seed);
            });

            var button = MenuCreation._buttonCreate("Generate Voronoi", () =>
            {
                Debug.Log($"Voronoi: size={_size}, sites={_siteCount}, seed={_seed}");
                try
                {
#if UNITY_EDITOR
                    var go = VoronatorUsageWrapper.CreateCuttedMeshAndSpawnInScene(_siteCount, _size, _seed);
                    if (go == null)
                        Debug.LogError("Voronoi: empty mesh (no clipped cells).");
                    else
                        VoronatorUsageWrapper.SpawnVoronoiInternalBorderLinesMeshInScene(
                            _siteCount, _size, _seed, go.transform);
#else
                    VoronatorUsageWrapper.CreateMesh(_siteCount, _size, _seed);
#endif
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
            });

            rootVisualElement.Add(sizeField);
            rootVisualElement.Add(countField);
            rootVisualElement.Add(seedField);
            rootVisualElement.Add(button);
        }
    }
}
