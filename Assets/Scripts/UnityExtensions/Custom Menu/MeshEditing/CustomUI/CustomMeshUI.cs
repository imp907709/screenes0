using Meshes.ManualMesh;
using UnityEngine.UIElements;

namespace UnityExtensions.Custom_Menu.MeshEditing.CustomUI
{
    public static class CustomMeshUI
    {
        public static void CustomMeshUIAdd(VisualElement rootVisualElement)
        {
            var button = MenuCreation._buttonCreate("Generate custom mesh", () =>
            {
                ManualMeshController.GO();
            });
            
            rootVisualElement.Add(button);
        }
    }
}