using System;
using System.Collections.Generic;
using System.Linq;
using Meshes.GeneralMesh;
using UnityEngine;
using Math = Unity.Mathematics.Geometry.Math;
using Random = System.Random;

namespace Meshes.ManualMesh
{
    // manually create mesh
    public class ManualMesh
    {
        // Applyes vertices and triangles producing new mesh
        public static Mesh Apply(List<Vector3> verts, List<int> tris, string name =  "CustomMesh")
        {
            var mesh = new Mesh { name = name};
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
        
    }
}