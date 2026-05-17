using System.Collections.Generic;
using Meshes.GeneralMesh;
using UnityEngine;
using UnityEngine.Rendering;

namespace Meshes.ManualMesh
{
    public class MeshDebug
    {
        public static void EraseList(List<GameObject> go)
        {
            Debug.Log("EraseList");
            if (go == null || go.Count == 0)
                return;

            for (int i = 0; i < go.Count; i++)
            {
                var obj = go[i];
                if (obj == null)
                    continue;
                
                MeshDebug.EraseObj(obj);
            }

            go.Clear();
        }
        
        public static void EraseObj(GameObject obj)
        {
            Debug.Log("EraseObj");
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
            Debug.Log("CreateSphereObjectsFromVerts");
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
        
        /// <summary>
        /// Combines meshes from existing scene objects (e.g. after <see cref="MeshDebug.CreateSphereObjectsFromVerts"/>).
        /// Optionally destroys sources and clears <paramref name="sources"/>.
        /// </summary>
        public static GameObject MergeGameObjectsIntoOne(
            List<GameObject> sources,
            bool destroySources = true,
            string objectName = "mergedMesh",
            Material materialOverride = null)
        {
            Debug.Log("MergeGameObjectsIntoOne");
            if (sources == null || sources.Count == 0)
                return null;

            var combines = new List<CombineInstance>();
            Material material = materialOverride;

            for (int i = 0; i < sources.Count; i++)
            {
                var src = sources[i];
                if (src == null)
                    continue;

                var filter = src.GetComponent<MeshFilter>();
                if (filter == null || filter.sharedMesh == null)
                    continue;

                combines.Add(new CombineInstance
                {
                    mesh = filter.sharedMesh,
                    transform = src.transform.localToWorldMatrix
                });

                if (material == null)
                {
                    var renderer = src.GetComponent<MeshRenderer>();
                    if (renderer != null)
                        material = renderer.sharedMaterial;
                }
            }

            if (combines.Count == 0)
                return null;

            // CombineMeshes often still enforces 16-bit indices; merge verts/tris manually (see MeshBlob.MergeHexMeshes).
            var mergedVerts = new List<Vector3>();
            var mergedTris = new List<int>();
            int vertexOffset = 0;

            for (int i = 0; i < combines.Count; i++)
            {
                var mesh = combines[i].mesh;
                var matrix = combines[i].transform;
                var meshVerts = mesh.vertices;
                var meshTris = mesh.triangles;

                for (int v = 0; v < meshVerts.Length; v++)
                    mergedVerts.Add(matrix.MultiplyPoint3x4(meshVerts[v]));

                for (int t = 0; t < meshTris.Length; t++)
                    mergedTris.Add(meshTris[t] + vertexOffset);

                vertexOffset += meshVerts.Length;
            }

            var mergedMesh = new Mesh
            {
                name = objectName,
                indexFormat = IndexFormat.UInt32
            };
            mergedMesh.SetVertices(mergedVerts);
            mergedMesh.SetTriangles(mergedTris, 0);
            mergedMesh.RecalculateNormals();
            mergedMesh.RecalculateBounds();

            material ??= MaterialFactory.GetBiomeVertexColorMaterial() ?? MaterialFactory.GetDefaultMaterial();

            var result = MeshObjectFactory.Create(mergedMesh, material, objectName);

            if (destroySources)
                MeshDebug.EraseList(sources);

            return result;
        }
    }
}