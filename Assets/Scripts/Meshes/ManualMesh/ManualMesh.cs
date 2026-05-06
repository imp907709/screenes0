using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Meshes.ManualMesh
{
    // manually create mesh
    public class ManualMesh
    {
        public static Mesh Apply(List<Vector3> verts, List<int> tris, string name =  "CustomMesh")
        {
            var mesh = new Mesh { name = name};
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
        
        public static Mesh CreateTrianlge()
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
            
            return Apply(verts, tris, "CustomTriangle");
        }
        
        public static Mesh CreateOctahedron(float radius = 1f)
        {
            List<Vector3> verts = new List<Vector3>();

            // 0–7 ring
            for (int i = 0; i < 8; i++)
            {
                float angle = i * Mathf.PI * 2f / 8f;

                verts.Add(new Vector3(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius,
                    0f
                ));
            }

            // ADD CENTER (this was missing)
            verts.Add(Vector3.zero); // index 8

            List<int> tris = new List<int>();

            int centerIndex = 8;

            for (int i = 0; i < 8; i++)
            {
                int next = (i + 1) % 8;

                tris.Add(centerIndex);
                tris.Add(i);
                tris.Add(next);
            }

            return Apply(verts, tris, "CustomOctahedron");
        }

        public static Mesh CreateHexagon(float radius = 1f)
        {
            var vecs =  new List<Vector3>();

            for (int i = 0; i < 6; i++)
            {
                // 2p / edges
                float angle = i * Mathf.PI * 2f / 6f;
                var v = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
                vecs.Add(v);
            }
            
            // zero center
            vecs.Add(Vector3.zero);
            var centerIndex = 6;
            
            var tris = new List<int>();

            for (int i = 0; i < 6; i++)
            {
                int next = (i + 1) % 6;
                
                tris.Add(centerIndex);
                tris.Add(i);
                tris.Add(next);
            }
            return Apply(vecs, tris, "Hexagon");
        }
    }
}