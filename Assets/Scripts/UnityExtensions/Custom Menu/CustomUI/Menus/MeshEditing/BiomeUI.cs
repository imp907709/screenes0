using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityExtensions.Custom_Menu.Core;

namespace UnityExtensions.Custom_Menu.CustomUI.MeshEditing
{
    public class BiomeUI
    {
        public const int DefaultGridWidth = 12;
        public const int DefaultGridDepth = 12;
        public const float DefaultCellSpacing = 1f;
        
        public static void CustomMeshUIAdd(VisualElement rootVisualElement)
        {
            var _width = CreateTextField.CreateInt(DefaultGridWidth, "Width");
            var _depth = CreateTextField.CreateInt(DefaultGridDepth, "Depth");
            var _spacing =  CreateTextField.CreateFloat(DefaultCellSpacing, "Spacing");
            
            var button = MenuCreation._buttonCreate("Generate biomes", () =>
            {
                try
                {
                    int width = CreateTextField.ParseIntField(_width, DefaultGridWidth, min: 1, max: 512);
                    int depth = CreateTextField.ParseIntField(_depth, DefaultGridDepth, min: 1, max: 512);
                    float spacing = CreateTextField.ParseFloatField(_spacing, DefaultCellSpacing, min: 0.0001f, max: 1e6f);

                    var (world, mesh) = WrapperGenerator.GenerateSampleWorldWithMesh(
                        width: width,
                        depth: depth,
                        spacing: spacing,
                        randomSeed: null);
                    
                    Debug.Log(
                        $"BiomeUI: WrapperGenerator done — grid={width}x{depth} spacing={spacing}, " +
                        $"cells={world.Cells.Count}, mesh verts={mesh.vertexCount}, tris={mesh.triangles.Length / 3}");

                    GameObject spawned = WrapperGenerator.SpawnSampleWorldMeshInScene(mesh);
                    if (spawned != null)
                        Debug.Log($"BiomeUI: spawned scene object '{spawned.name}'.");
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            });

            rootVisualElement.Add(_width);
            rootVisualElement.Add(_depth);
            rootVisualElement.Add(_spacing);
            rootVisualElement.Add(button);
        }
    }
}