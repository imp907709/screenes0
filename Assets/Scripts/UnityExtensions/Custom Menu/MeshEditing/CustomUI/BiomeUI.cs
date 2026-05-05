using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityExtensions.Custom_Menu.MeshEditing.CustomUI
{
    public class BiomeUI
    {
        public static void CustomMeshUIAdd(VisualElement rootVisualElement)
        {
            var button = MenuCreation._buttonCreate("Generate biomes", () =>
            {
                try
                {
                    var (world, mesh) = WrapperGenerator.GenerateSampleWorldWithMesh(randomSeed: null);
                    Debug.Log(
                        $"BiomeUI: WrapperGenerator done — cells={world.Cells.Count}, " +
                        $"mesh verts={mesh.vertexCount}, tris={mesh.triangles.Length / 3}");

                    GameObject spawned = WrapperGenerator.SpawnSampleWorldMeshInScene(mesh);
                    if (spawned != null)
                        Debug.Log($"BiomeUI: spawned scene object '{spawned.name}'.");
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            });

            rootVisualElement.Add(button);
        }
    }
}