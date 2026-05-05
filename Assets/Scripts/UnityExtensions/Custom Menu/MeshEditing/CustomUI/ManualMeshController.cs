using Meshes.GeneralMesh;
using Meshes.ManualMesh;
using UnityEngine;

namespace UnityExtensions.Custom_Menu.MeshEditing.CustomUI
{
    public class ManualMeshController
    {
        public static void GO()
        {
            var mesh = ManualMesh.CreateCustomMesh();
            var material = MaterialFactory.GetDefault();
            var go = MeshObjectFactory.Create(mesh, material);

            go.transform.position = Vector3.zero;
        }
    }
}