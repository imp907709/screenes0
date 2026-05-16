using System;
using System.Collections.Generic;
using System.Linq;
using Meshes.GeneralMesh;
using Meshes.Voronoi.VoronatorUsage;
using UnityEngine;
using Math = Unity.Mathematics.Geometry.Math;
using Random = System.Random;

namespace Meshes.ManualMesh
{
    // manually create mesh
    public class MeshGeneral
    {
        // Applies vertices and triangles producing new mesh
        public static UnityEngine.Mesh Apply(List<Vector3> verts, List<int> tris, string name =  "CustomMesh")
        {
            var mesh = new UnityEngine.Mesh { name = name};
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        public static UnityEngine.Mesh MeshApply(UnityEngine.Mesh mesh, List<Vector3> verts)
        {
            mesh.SetVertices(verts);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        public static List<Vector3> CreatePlaneVerts(int width = 10, int height = 10, int resolution = 100)
        {
            var verts = new List<Vector3>();
         
            if(width < 1 || height < 1 || resolution < 1)
                return verts;

            int wCount = resolution;
            int hCount = resolution;

            float wStep = width / (float)(wCount - 1);
            float hStep = height / (float)(hCount - 1);

            Debug.Log($"CreatePlaneVerts COUNTS: {wCount}x{hCount}");
            Debug.Log($"CreatePlaneVerts STEPS W: {wStep}, H: {hStep}");

            for (int w = 0; w < wCount; w++)
            {
                for (int h = 0; h < hCount; h++)
                {
                    float wC = w * wStep;
                    float hC = h * hStep;

                    verts.Add(new Vector3(wC, 0, hC));
                }
            }
            
            Debug.Log($"CreatePlaneVerts : {verts.Count}");
            return verts;
        }

        public static List<int> CreatePlaneTris(List<Vector3> verts, int resolution = 100)
        {
            var tris = new List<int>();

            int vertsPerRow = resolution;

            for (int y = 0; y < vertsPerRow - 1; y++)
            {
                for (int x = 0; x < vertsPerRow - 1; x++)
                {
                    int a = y * vertsPerRow + x;
                    int b = a + 1;
                    int c = a + vertsPerRow;
                    int d = c + 1;

                    tris.Add(a);
                    tris.Add(b);
                    tris.Add(c);

                    tris.Add(c);
                    tris.Add(b);
                    tris.Add(d);
                }
            }
            
            return tris;
        }
    }
}