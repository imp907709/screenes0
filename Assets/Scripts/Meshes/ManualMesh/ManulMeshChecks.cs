using Meshes.GeneralMesh;
using UnityEngine;

namespace Meshes.ManualMesh
{
    public class ManulMeshChecks
    {
        public static GameObject CreatePlaneMeshObject()
        {
            var verts = ManualMesh.CreatePlaneVerts();
            var triags = ManualMesh.CreatePlaneTriags(verts);
            
            var mesh = ManualMesh.Apply(verts, triags);
            Material material = MaterialFactory.GetBiomeVertexColorMaterial() ?? MaterialFactory.GetDefaultMaterial();
            var go = MeshObjectFactory.Create(mesh, material, "planeMesh");
            
            return go;
        }
    }
}