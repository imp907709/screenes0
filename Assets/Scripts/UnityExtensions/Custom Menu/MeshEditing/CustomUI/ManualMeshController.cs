using Meshes.GeneralMesh;
using Meshes.ManualMesh;
using UnityEngine;

namespace UnityExtensions.Custom_Menu.MeshEditing.CustomUI
{
    public class ManualMeshController
    {
        public static void GO(int angles = 3, float radius = 1f)
        {
            // var mesh = ManualMesh.CreateOctahedron(radius);
            // var mesh = ManualMesh.CreateHexagon(radius);
            var mesh = ManualMesh.CreateAngled(angles);
            
            var material = MaterialFactory.GetDefaultMaterial();
            MaterialFactory.ApplyRandomColor(material);
            var go = MeshObjectFactory.Create(mesh, material);

            go.transform.position = Vector3.zero;
        }
    }
}