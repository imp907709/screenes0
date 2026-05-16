using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityExtensions.Custom_Menu.Core;
using UnityExtensions.Custom_Menu.CustomUI.Controllers;

namespace UnityExtensions.Custom_Menu.CustomUI.Menus.MeshEditing
{
    public class ManualUI
    {
        public static void Init(VisualElement root)
        {
            // elements in a row
            var row = new VisualElement { style = { flexDirection = FlexDirection.Row } };
            // label
            row.Add(new Label("Test event"));
            // button with event
            var button = new Button(() => ManualController.GO()) { text = "Effect" };
            
            var slilder = MenuCreation._sliderMenu("Margin");
            
            row.Add(button);
           
            root.Add(row);
            
            root.Add(slilder);
        }
    }
}