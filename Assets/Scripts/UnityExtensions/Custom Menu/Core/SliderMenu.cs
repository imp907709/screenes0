using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityExtensions.Custom_Menu.MeshEditing
{
    public class MenuCreation
    {
        public static void Create(VisualElement root)
        {
            var slider = _sliderMenu(
                "Size",
                0.1f,
                10f,
                value =>
                {
                    if (MeshEditorCubeModel._cubeMesh == null)
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
                        MeshEditorCubeModel._cubeMesh.Generate(value);
                }
            );
            
            root.Add(slider);
        }

        public static Slider _sliderMenu( string label = "Slider", float min = 1, float max = 10, Action<float> onChanged = default)
        {
            var slider = new Slider(label, min, max);
            slider.value = min;

            slider.RegisterValueChangedCallback(evt =>
            {
                onChanged?.Invoke(evt.newValue);
            });

            return slider;
        }
        
        public static Button _buttonCreate(string label, System.Action onClick)
        {
            var button = new Button(() =>
            {
                Debug.Log($"{label} clicked");
                onClick?.Invoke();
            });

            button.text = label;
            return button;
        }
    }
}