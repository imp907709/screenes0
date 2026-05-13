using System.Collections.Generic;
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
            var res = 100;
            var rect = res / 2;
            
            var verts = ManualMesh.CreatePlaneVerts(50,50,res);
            var triags = ManualMesh.CreatePlaneTris(verts,res);
            
            verts = MeshBlob.AddNoise(verts, 12, 0.08f);
            verts = MeshBlob.AddNoise(verts, 0.7f, 110);
            
            // MeshDebug.CreateSphereObjectsFromVerts(verts);
            
            var mesh = ManualMesh.Apply(verts, triags);
            Material material = MaterialFactory.GetBiomeVertexColorMaterial() ?? MaterialFactory.GetDefaultMaterial();
            var go = MeshObjectFactory.Create(mesh, material, "planeMesh");
            
            return go;
        }

        /// <summary>
        /// Same plane vertex grid as <see cref="CreatePlaneMeshObject"/> before noise — for UI refresh that reapplies hill + detail onto the existing mesh without clearing Y.
        /// </summary>
        public static List<Vector3> CreateFlatPlaneVertexBaselineForNoiseUi()
        {
            const int res = 100;
            var verts = ManualMesh.CreatePlaneVerts(50, 50, res);
            return new List<Vector3>(verts);
        }
    }
}