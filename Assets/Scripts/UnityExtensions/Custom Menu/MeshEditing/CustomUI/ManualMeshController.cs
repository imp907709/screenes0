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
            // var mesh = ManualMesh.CreateAngled(angles);
            // var mesh = ManualMesh.CreatePlane();
            // var mesh = ManualMesh.CreatePlaneAdjusted();
            // GameObject go = MeshExamples.CreateHexGridMeshObject();
            
            GameObject go = ManulMeshChecks.CreatePlaneMeshObject();
            go.transform.position = Vector3.zero;
        }
    }
}