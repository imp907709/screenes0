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
        
        // generalization for angled
        // min 3 edges
        // qube 4, hex 6, oct 8, circles ~> 25
        public static Mesh CreateAngled(int vertices = 3, float radius = 1f )
        {
            var vecs =  new List<Vector3>();

            for (int i = 0; i < vertices; i++)
            {
                // 2p / edges
                float angle = i * Mathf.PI * 2f / vertices;
                var v = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
                vecs.Add(v);
            }
            
            // zero center
            vecs.Add(Vector3.zero);
            var centerIndex = vertices;
            
            var tris = new List<int>();

            for (int i = 0; i < vertices; i++)
            {
                int next = (i + 1) % vertices;
                
                tris.Add(centerIndex);
                tris.Add(i);
                tris.Add(next);
            }
            return Apply(vecs, tris, "Angled");
        }
        
        private static List<Vector3> debugVecs = new ();
        public static Mesh CreatePlane(int width = 10, int depth = 10, float resolution = 10)
        {
            var vecs = new List<Vector3>();
            
            float stepX = (float)width / (resolution - 1);
            float stepZ = (float)depth / (resolution - 1);

            for (int x = 0; x < resolution; x++)
            {
                for (int z = 0; z < resolution; z++)
                {
                    float px = x * stepX;
                    float pz = z * stepZ;
                    
                    vecs.Add(new Vector3(px,0,pz));
                }
            }

            var tris = new List<int>();

            if (tris?.Any() != true)
            {
                debugVecs = vecs;
                return CreateVertexDebugMesh(verts:vecs);
            }
            
            return Apply(vecs, tris, "Plane");
        }
        
        void OnDrawGizmos()
        {
            if (debugVecs == null) return;

            Gizmos.color = Color.red;

            foreach (var v in debugVecs)
            {
                Gizmos.DrawSphere(v, 0.1f);
            }
        }
        
        public static Mesh CreateVertexDebugMesh(List<Vector3> verts, float size = 0.05f)
        {
            var v = new List<Vector3>();
            var tris = new List<int>();

            foreach (var p in verts)
            {
                int startIndex = v.Count;

                // simple "cross quad" (cheap visible marker)

                v.Add(p + new Vector3(-size, 0, -size));
                v.Add(p + new Vector3(size, 0, -size));
                v.Add(p + new Vector3(size, 0, size));
                v.Add(p + new Vector3(-size, 0, size));

                tris.Add(startIndex + 0);
                tris.Add(startIndex + 1);
                tris.Add(startIndex + 2);

                tris.Add(startIndex + 2);
                tris.Add(startIndex + 3);
                tris.Add(startIndex + 0);
            }

            Mesh m = new Mesh();
            m.SetVertices(v);
            m.SetTriangles(tris, 0);
            m.RecalculateNormals();
            m.RecalculateBounds();

            return m;
        }
    }
}