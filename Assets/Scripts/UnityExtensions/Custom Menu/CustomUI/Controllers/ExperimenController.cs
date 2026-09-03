using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityExtensions.Custom_Menu.Core;
using UnityExtensions.Custom_Menu.CustomUI.Controllers;
using UnityExtensions.Custom_Menu.CustomUI.Menus.MeshEditing;

namespace UnityExtensions.Custom_Menu.Utils
{
    public class ExperimenController
    {
 
        private static ExperimantService _service = new ();

        public static void CreateIntValueField(VisualElement root, int? _intDefault = 0)
        {
            var _width = CreateTextField.CreateInt((int)_intDefault, "Value", OnValueChanged);
 
            root.Add(_width);
        }
        public static void OnValueChanged(ChangeEvent<string> changed)
        {
            var _valueChanged = 0;
            int.TryParse(changed.newValue, out _valueChanged);
            _service.Value =  _valueChanged;
        }
        
        
        public static void CreateSlider(VisualElement root)
        {
            var slider = MenuCreation._sliderMenu("Slide variable", 0F,10F, OnSliderValueChange);
            root.Add(slider);
        }
        public static void OnSliderValueChange(float value)
        {
            _service.sliderVal = value;
            // Debug.Log($"Received: {_service.sliderVal}");
        }

        public static void Reset()
        {
            _service.Reset();
        }

        
        public static void CreateButton(VisualElement root)
        {
            var _btn = MenuCreation._buttonCreate("Create", CreateMesh);
            root.Add(_btn);
        }
        public static void CreateMesh()
        {
            Debug.Log($"CreateMesh width: {_service.sliderVal}, {_service.Value}");
            // ManualController.GO();
        }

    }
}