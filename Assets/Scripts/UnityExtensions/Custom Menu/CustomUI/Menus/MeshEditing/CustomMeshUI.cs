using System.Globalization;
using UnityEngine.UIElements;
using UnityExtensions.Custom_Menu.Core;
using UnityExtensions.Custom_Menu.CustomUI.Controllers;

namespace UnityExtensions.Custom_Menu.CustomUI.Menus.MeshEditing
{
    public static class CustomMeshUI
    {
        public static float _scale = 1.0f;
        public static int _vertices = 3;
        /// <summary>Used for Generate (GO) and as the single live draft noise layer (one AddNoise per UI change until Save).</summary>
        public static float _draftAmplitude = 0.5f;
        public static float _draftFrequency = 80f;

        public static void CustomMeshUIAdd(VisualElement rootVisualElement)
        {
            var scaleField = CreateTextField.CreateFloat(_scale, "SCale");
            scaleField.RegisterValueChangedCallback(s => {
                float.TryParse(s.newValue, NumberStyles.Float, CultureInfo.InvariantCulture, out _scale);
            });
          
            var draftAmplitudeField = CreateTextField.CreateFloat(_draftAmplitude, "Draft noise amplitude");
            var draftFrequencyField = CreateTextField.CreateFloat(_draftFrequency, "Draft noise frequency");

            void Refresh() => RefreshPlaneNoiseIfBound(draftAmplitudeField, draftFrequencyField);

            draftAmplitudeField.RegisterValueChangedCallback(_ => Refresh());
            draftFrequencyField.RegisterValueChangedCallback(_ => Refresh());

            var button = MenuCreation._buttonCreate("Generate custom mesh", () =>
            {

                float amp = CreateTextField.ParseFloatField(draftAmplitudeField, _draftAmplitude, min: 0f, max: 1e6f);
                float freq = CreateTextField.ParseFloatField(draftFrequencyField, _draftFrequency, min: 0.0001f, max: 1e6f);
                _draftAmplitude = amp;
                _draftFrequency = freq;

                ManualMeshController.GO(amp, freq);
            });

            var saveLayerButton = MenuCreation._buttonCreate("Save noise layer", () =>
            {
                float amp = CreateTextField.ParseFloatField(draftAmplitudeField, _draftAmplitude, min: 0f, max: 1e6f);
                float freq = CreateTextField.ParseFloatField(draftFrequencyField, _draftFrequency, min: 0.0001f, max: 1e6f);
                _draftAmplitude = amp;
                _draftFrequency = freq;
                ManualMeshController.SaveCommittedNoiseLayerFromActiveMesh();
                ManualMeshController.SetDraftNoiseTrackingFromUi(amp, freq);
            });

            rootVisualElement.Add(scaleField);
            rootVisualElement.Add(draftAmplitudeField);
            rootVisualElement.Add(draftFrequencyField);
            rootVisualElement.Add(button);
            rootVisualElement.Add(saveLayerButton);
        }

        static void RefreshPlaneNoiseIfBound(TextField draftAmplitudeField, TextField draftFrequencyField)
        {
            if (ManualMeshController.ActivePlaneMeshObject == null)
                return;

            float amp = CreateTextField.ParseFloatField(draftAmplitudeField, _draftAmplitude, min: 0f, max: 1e6f);
            float freq = CreateTextField.ParseFloatField(draftFrequencyField, _draftFrequency, min: 0.0001f, max: 1e6f);
            _draftAmplitude = amp;
            _draftFrequency = freq;

            var go = ManualMeshController.ActivePlaneMeshObject;
            ManualMeshController.UpdateExistingMeshDraftSingleNoise(go, amp, freq);
        }
    }
}
