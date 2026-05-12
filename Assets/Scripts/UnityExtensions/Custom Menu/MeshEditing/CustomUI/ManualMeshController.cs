using Meshes.GeneralMesh;
using Meshes.ManualMesh;
using UnityEngine;

namespace UnityExtensions.Custom_Menu.MeshEditing.CustomUI
{
    public class ManualMeshController
    {
        public static void GO(float amp, float freq)
        {
            // var mesh = ManualMesh.CreateOctahedron(radius);
            // var mesh = ManualMesh.CreateHexagon(radius);
            // var mesh = ManualMesh.CreateAngled(angles);
            // var mesh = ManualMesh.CreatePlane();
            // var mesh = ManualMesh.CreatePlaneAdjusted();
            // GameObject go = MeshExamples.CreateHexGridMeshObject();
            
            GameObject go = ManualMeshChecks.CreatePlaneMeshObject(amp,freq);
            go.transform.position = Vector3.zero;
        }
    }
}