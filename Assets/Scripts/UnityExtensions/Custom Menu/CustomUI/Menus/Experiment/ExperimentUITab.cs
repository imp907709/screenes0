using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using UnityExtensions.Custom_Menu.Core;
using UnityExtensions.Custom_Menu.Utils;

namespace UnityExtensions.Custom_Menu.CustomUI.Menus.Experiment
{
    public class ExperimentUITab
    {
        
        public const int _intDefault  = 0;
        
        public static void ExperimentMenuUI(VisualElement root)
        {
            // width int textbox
            ExperimenController.CreateIntValueField(root, _intDefault);

            ExperimenController.CreateSlider(root);

            ExperimenController.CreateButton(root);
        }
      
    }
}