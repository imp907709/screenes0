using UnityEngine;
using UnityExtensions.Custom_Menu.CustomUI.Menus.MeshEditing;

namespace UnityExtensions.Custom_Menu.Utils
{
    public class ExperimantService
    {
 
        private float sliderInit = 0f;
        public float sliderVal = 0f;
   
        public void Reset()
        {
            sliderVal = sliderInit;
        }
        
        public int Value { get; set; }

    }
}