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

        public static List<Vector3> CreatePlaneVerts(int width = 10, int height = 10, int resolution = 100)
        {
            var verts = new List<Vector3>();
         
            if(width < 1 || height < 1 || resolution < 1)
                return verts;
            
            var wStep = width / resolution;
            var hStep = height / resolution;

            for (int i = 0; i < width; i+=wStep)
                for (int i2 = 0; i2 < height; i2+=hStep)
                    verts.Add(new Vector3(i, 0, i2));
            
            return verts;
        }

        public static List<int> CreatePlaneTriags(List<Vector3> verts)
        {
            
            var triags =  new List<int>();
            var len = verts.Count;
            var res = len / 2;

            for (int i = 0; i < res; i++)
            {
                for (int i2 = 0; i2 < res; i2++)
                {
                    
                }
            }
            
            return triags;
        }
    }
}