using Meshes;
using UnityEngine;
using UnityEngine.UIElements;
using UnityExtensions.Custom_Menu.Core;

namespace UnityExtensions.Custom_Menu.CustomUI.Menus.MeshEditing
{
    public class QubeCreatorUI
    {
        public static void Create(VisualElement root)
        {
            var slider = MenuCreation._sliderMenu(
                "Size",
                0.1f,
                10f,
                value =>
                {
                    if (MeshEditorCubeModel._mesh == null)
                    {
                        Debug.Log("No generator");
                        return;
                    }
       
                    if (MeshEditorCubeModel._meshFilter == null)
                    {
                        Debug.Log("No mesh filter");
                        return;
                    }
       
                    Debug.Log("Mesh generator applied");
       
                    MeshEditorCubeModel._meshFilter.sharedMesh =
                        CubeMeshService.Generate(value);
                }
            );
            
            root.Add(slider);
        }
    }
}