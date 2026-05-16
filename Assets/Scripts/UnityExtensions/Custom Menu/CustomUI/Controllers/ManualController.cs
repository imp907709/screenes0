using System.Collections.Generic;
using Meshes.ManualMesh;
using UnityEngine;

namespace UnityExtensions.Custom_Menu.CustomUI.Controllers
{
    public class ManualController
    {
        private static GameObject go;
        public static void ButtonClickedDebug()
        {
            Debug.Log("ButtonClickedDebug");
        }

        public static void GO()
        {
            Debug.Log("GO");
       
            go = ManualMeshChecks.ManualMeshToCheck(go);
        }
    }
}