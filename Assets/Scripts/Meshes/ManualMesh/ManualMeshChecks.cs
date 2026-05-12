using Meshes.GeneralMesh;
using UnityEngine;

namespace Meshes.ManualMesh
{
    public class ManualMeshChecks
    {
        /// <summary>
        /// Generating actual mesh
        /// </summary>
        public static GameObject CreatePlaneMeshObject(float amplitude, float frequency)
        {
            var res = 10;
            var rect = res / 2;
            
            var verts = ManualMesh.CreatePlaneVerts(rect,rect,res);
            var triags = ManualMesh.CreatePlaneTris(verts,res);
            
            verts = MeshBlob.AddNoise(verts, amplitude, frequency);
            
            MeshDebug.CreateSphereObjectsFromVerts(verts);
            
            var mesh = ManualMesh.Apply(verts, triags);
            Material material = MaterialFactory.GetBiomeVertexColorMaterial() ?? MaterialFactory.GetDefaultMaterial();
            var go = MeshObjectFactory.Create(mesh, material, "planeMesh");
            
            return go;
        }
    }
}