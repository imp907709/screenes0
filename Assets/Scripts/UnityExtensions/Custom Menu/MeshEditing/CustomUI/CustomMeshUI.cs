using System.Globalization;
using Meshes.ManualMesh;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityExtensions.Custom_Menu.MeshEditing.CustomUI
{
    public static class CustomMeshUI
    {
        public static float _scale = 1.0f;
        public static int _vertices = 3;
        public static float _amplitude = 0.5f;
        public static float _frequency = 80f;

        public static void CustomMeshUIAdd(VisualElement rootVisualElement)
        {
            var scaleField = new TextField("Radius");
            scaleField.value = _scale.ToString(CultureInfo.InvariantCulture);
            scaleField.RegisterValueChangedCallback(evt =>
            {
                float.TryParse(evt.newValue, NumberStyles.Float, CultureInfo.InvariantCulture, out _scale);
            });

            var vertices = CreateTextField.CreateInt(_vertices, "Vertices");
            var amplitudeField = CreateTextField.CreateFloat(_amplitude, "Amplitude");
            var frequencyField = CreateTextField.CreateFloat(_frequency, "Frequency");

            amplitudeField.RegisterValueChangedCallback(_ => RefreshPlaneNoiseIfBound(amplitudeField, frequencyField));
            frequencyField.RegisterValueChangedCallback(_ => RefreshPlaneNoiseIfBound(amplitudeField, frequencyField));

            var button = MenuCreation._buttonCreate("Generate custom mesh", () =>
            {
                _vertices = CreateTextField.ParseIntField(vertices, _vertices, min: 1, max: 512);
                _scale = CreateTextField.ParseFloatField(scaleField, _scale, min: 0.0001f, max: 1e6f);

                float amp = CreateTextField.ParseFloatField(amplitudeField, _amplitude, min: 0f, max: 1e6f);
                float freq = CreateTextField.ParseFloatField(frequencyField, _frequency, min: 0.0001f, max: 1e6f);
                _amplitude = amp;
                _frequency = freq;

                ManualMeshController.GO(amp, freq);
            });

            rootVisualElement.Add(scaleField);
            rootVisualElement.Add(vertices);
            rootVisualElement.Add(amplitudeField);
            rootVisualElement.Add(frequencyField);
            rootVisualElement.Add(button);
        }

        static void RefreshPlaneNoiseIfBound(TextField amplitudeField, TextField frequencyField)
        {
            if (ManualMeshController.ActivePlaneMeshObject == null)
                return;

            float amp = CreateTextField.ParseFloatField(amplitudeField, _amplitude, min: 0f, max: 1e6f);
            float freq = CreateTextField.ParseFloatField(frequencyField, _frequency, min: 0.0001f, max: 1e6f);
            _amplitude = amp;
            _frequency = freq;

            ManualMeshController.ApplyAddNoiseToExistingMesh(ManualMeshController.ActivePlaneMeshObject, amp, freq);
        }
    }
}
