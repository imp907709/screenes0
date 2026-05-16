using System.Collections.Generic;
using Meshes.GeneralMesh;
using UnityEngine;

namespace Meshes.ManualMesh
{
    public class MeshDebug
    {
        public static void EraseObj(GameObject obj)
        {
            #if UNITY_EDITOR
                if (!Application.isPlaying)
                    UnityEngine.Object.DestroyImmediate(obj);
                else
                    UnityEngine.Object.Destroy(obj);
            #else
                        Object.Destroy(obj);
            #endif
        }
        
        // creates sample square around point
        public static List<GameObject> CreateSphereObjectsFromVerts(
            List<Vector3> centers,
            float radius = 0.1f,
            int latitudeSegments = 8,
            int longitudeSegments = 8)
        {
            var result = new List<GameObject>();

            foreach (var center in centers)
            {
                var verts = new List<Vector3>();
                var tris = new List<int>();

                for (int lat = 0; lat <= latitudeSegments; lat++)
                {
                    float a1 = Mathf.PI * lat / latitudeSegments;
                    float sin1 = Mathf.Sin(a1);
                    float cos1 = Mathf.Cos(a1);

                    for (int lon = 0; lon <= longitudeSegments; lon++)
                    {
                        float a2 = 2f * Mathf.PI * lon / longitudeSegments;
                        float sin2 = Mathf.Sin(a2);
                        float cos2 = Mathf.Cos(a2);

                        float x = sin1 * cos2;
                        float y = cos1;
                        float z = sin1 * sin2;

                        verts.Add(center + new Vector3(x, y, z) * radius);
                    }
                }

                int row = longitudeSegments + 1;

                for (int lat = 0; lat < latitudeSegments; lat++)
                {
                    for (int lon = 0; lon < longitudeSegments; lon++)
                    {
                        int a = lat * row + lon;
                        int b = a + 1;
                        int c = a + row;
                        int d = c + 1;

                        tris.Add(a);
                        tris.Add(b);
                        tris.Add(c);

                        tris.Add(c);
                        tris.Add(b);
                        tris.Add(d);
                    }
                }

                var mesh = MeshGeneral.Apply(verts, tris);

                Material material =
                    MaterialFactory.GetBiomeVertexColorMaterial()
                    ?? MaterialFactory.GetDefaultMaterial();

                var go = MeshObjectFactory.Create(mesh, material, "sphere");

                result.Add(go);
            }

            return result;
        }
        
    }
}