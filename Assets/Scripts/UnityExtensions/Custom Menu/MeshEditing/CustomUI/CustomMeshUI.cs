using System.Globalization;
using Meshes.ManualMesh;
using UnityEngine.UIElements;

namespace UnityExtensions.Custom_Menu.MeshEditing.CustomUI
{
    public static class CustomMeshUI
    {
        public static float _scale = 1.0f;
        public static int _vertices = 3;

        public static void CustomMeshUIAdd(VisualElement rootVisualElement)
        {
            
            var scaleField = new TextField("Radius");
            scaleField.value = _scale.ToString(CultureInfo.InvariantCulture);
            scaleField.RegisterValueChangedCallback(evt =>
            {
                float.TryParse(evt.newValue, NumberStyles.Float, CultureInfo.InvariantCulture, out _scale);
            });
            
            var vertices = CreateTextField.CreateInt(_vertices, "Vertices");

            
            var button = MenuCreation._buttonCreate("Generate custom mesh", () =>
            {
                int vert = CreateTextField.ParseIntField(vertices, _vertices, min: 1, max: 512);
                
                ManualMeshController.GO(vert,_scale);
            });
            
            rootVisualElement.Add(scaleField);
            rootVisualElement.Add(vertices);
            rootVisualElement.Add(button);
        }
    }
}