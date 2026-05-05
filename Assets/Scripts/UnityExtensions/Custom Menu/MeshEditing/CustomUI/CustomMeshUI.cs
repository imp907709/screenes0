using System.Globalization;
using Meshes.ManualMesh;
using UnityEngine.UIElements;

namespace UnityExtensions.Custom_Menu.MeshEditing.CustomUI
{
    public static class CustomMeshUI
    {
        private static float _scale = 1.0f;
        
        public static void CustomMeshUIAdd(VisualElement rootVisualElement)
        {
            var scaleField = new TextField("Radius");
            scaleField.value = _scale.ToString(CultureInfo.InvariantCulture);
            scaleField.RegisterValueChangedCallback(evt =>
            {
                float.TryParse(evt.newValue, NumberStyles.Float, CultureInfo.InvariantCulture, out _scale);
            });
            
            var button = MenuCreation._buttonCreate("Generate custom mesh", () =>
            {
                ManualMeshController.GO(_scale);
            });
            
            rootVisualElement.Add(scaleField);
            rootVisualElement.Add(button);
        }
    }
}