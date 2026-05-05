using System.Collections.Generic;
using UnityEngine;

namespace Meshes.ManualMesh
{
    public class ManualMesh
    {
        public static Mesh CreateCustomMesh()
        {
            var verts = new List<Vector3>();
            var tris = new List<int>();
            
            verts.Add(new Vector3(0.0f, 0.0f, 0.0f));
            verts.Add(new Vector3(0.0f, 1.0f, 1.0f));
            verts.Add(new Vector3(1.0f, 1.0f, 0.0f));
      
            tris.Add(0);
            tris.Add(1);
            tris.Add(2);
            
            tris.Add(2);
            tris.Add(1);
            tris.Add(0);
            
            var mesh = new Mesh { name = "CustomMesh" };
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}