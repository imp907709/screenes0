using Meshes.GeneralMesh;
using Meshes.ManualMesh;
using UnityEngine;

namespace UnityExtensions.Custom_Menu.MeshEditing.CustomUI
{
    public class ManualMeshController
    {
        public static void GO(float radius = 1f)
        {
            var mesh = ManualMesh.CreateOctahedron(radius);
            var material = MaterialFactory.GetDefaultMaterial();
            MaterialFactory.ApplyRandomColor(material);
            var go = MeshObjectFactory.Create(mesh, material);

            go.transform.position = Vector3.zero;
        }
    }
}