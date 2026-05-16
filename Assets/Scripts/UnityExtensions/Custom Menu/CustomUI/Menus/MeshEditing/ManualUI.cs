using UnityEngine.UIElements;
using UnityExtensions.Custom_Menu.Core;

namespace UnityExtensions.Custom_Menu.CustomUI.Menus.MeshEditing
{
    public class ManualUI
    {
        public static void Init(VisualElement root)
        {

            var slilder = MenuCreation._sliderMenu();
            var button = MenuCreation._buttonCreate();
            
            root.Add(slilder);
            root.Add(button);
        }
    }
}