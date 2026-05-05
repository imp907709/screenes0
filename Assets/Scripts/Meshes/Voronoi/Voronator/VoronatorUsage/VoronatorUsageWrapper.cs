using System.Collections.Generic;
using System.Globalization;
using Meshes.Voronoi.VoronatorSharp;
using UnityEngine;

namespace Meshes.Voronoi.VoronatorUsage
{
    // Actual voronoi wrapper for UI usage
    // voronoy cells itself
    // and internal lines only
    public static class VoronatorUsageWrapper
    {
        /// <summary>Same as <see cref="CreateCuttedMesh"/> but uses an existing <see cref="Voronator"/> (no second build).</summary>
        public static Mesh CreateCuttedMeshFromVoronator(Voronator v, float y = 0f)
        {
            var verts = new List<Vector3>();
            var tris = new List<int>();
            AppendCuttedMeshFillFans(v, y, verts, tris);

            var mesh = new Mesh { name = "VoronoiCutFill" };
            if (verts.Count == 0)
            {
                mesh.SetVertices(new List<Vector3>());
                mesh.SetTriangles(new List<int>(), 0);
                return mesh;
            }

            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        /// <summary>
        /// Separate mesh: <see cref="MeshTopology.Lines"/> only. Cell–cell edges (clip-rect boundary omitted); each internal segment once via deduplication.
        /// </summary>
        public static Mesh CreateVoronoiInternalBorderLinesMesh(Voronator v, float yWorld = 0f)
        {
            float clipSpan = Mathf.Max(v.ClipMax.x - v.ClipMin.x, v.ClipMax.y - v.ClipMin.y);
            float eps = Mathf.Max(1e-5f, clipSpan * 1e-6f);
            float y = yWorld + Mathf.Max(0.001f, clipSpan * 0.0005f);

            var verts = new List<Vector3>();
            var indices = new List<int>();
            var seenEdges = new HashSet<string>();
            int nSites = v.Delaunator.Points.Count;
            for (int i = 0; i < nSites; i++)
            {
                var poly = v.GetClippedPolygon(i);
                if (poly == null || poly.Count < 3)
                    continue;

                int n = poly.Count;
                for (int j = 0; j < n; j++)
                {
                    Vector2 a = poly[j];
                    Vector2 b = poly[(j + 1) % n];
                    if (VoronoiEdgeOnClipBoundary(a, b, v.ClipMin, v.ClipMax, eps))
                        continue;

                    if (!seenEdges.Add(VoronoiInternalEdgeDedupKey(a, b)))
                        continue;

                    int i0 = verts.Count;
                    verts.Add(new Vector3(a.x, y, a.y));
                    verts.Add(new Vector3(b.x, y, b.y));
                    indices.Add(i0);
                    indices.Add(i0 + 1);
                }
            }

            var mesh = new Mesh { name = "VoronoiInternalLines" };
            if (verts.Count == 0)
            {
                mesh.SetVertices(new List<Vector3>());
                return mesh;
            }

            mesh.SetVertices(verts);
            mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
            ApplyLinesMeshUpNormals(mesh, verts.Count);
            mesh.RecalculateBounds();
            return mesh;
        }

        private static string VoronoiInternalEdgeDedupKey(Vector2 a, Vector2 b)
        {
            static string F(Vector2 p) =>
                p.x.ToString("G9", CultureInfo.InvariantCulture) + "," +
                p.y.ToString("G9", CultureInfo.InvariantCulture);

            string sa = F(a), sb = F(b);
            return string.CompareOrdinal(sa, sb) <= 0 ? sa + ";" + sb : sb + ";" + sa;
        }

        private static bool VoronoiEdgeOnClipBoundary(Vector2 a, Vector2 b, Vector2 clipMin, Vector2 clipMax, float eps)
        {
            bool onLeft = Mathf.Abs(a.x - clipMin.x) < eps && Mathf.Abs(b.x - clipMin.x) < eps;
            bool onRight = Mathf.Abs(a.x - clipMax.x) < eps && Mathf.Abs(b.x - clipMax.x) < eps;
            bool onBottom = Mathf.Abs(a.y - clipMin.y) < eps && Mathf.Abs(b.y - clipMin.y) < eps;
            bool onTop = Mathf.Abs(a.y - clipMax.y) < eps && Mathf.Abs(b.y - clipMax.y) < eps;
            return onLeft || onRight || onBottom || onTop;
        }

        private static void ApplyLinesMeshUpNormals(Mesh mesh, int vertexCount)
        {
            var normals = new Vector3[vertexCount];
            for (int i = 0; i < vertexCount; i++)
                normals[i] = Vector3.up;
            mesh.normals = normals;
        }

        private static void AppendCuttedMeshFillFans(Voronator v, float y, List<Vector3> verts, List<int> tris)
        {
            int nSites = v.Delaunator.Points.Count;
            for (int i = 0; i < nSites; i++)
            {
                var poly = v.GetClippedPolygon(i);
                if (poly == null || poly.Count < 3)
                    continue;
                AppendCuttedMeshCellFanCcw(verts, tris, poly, y);
            }
        }

        private static void AppendCuttedMeshCellFanCcw(List<Vector3> verts, List<int> tris, List<Vector2> poly, float y)
        {
            if (CuttedMeshSignedAreaXz(poly) < 0f)
            {
                var rev = new List<Vector2>(poly.Count);
                for (int k = poly.Count - 1; k >= 0; k--)
                    rev.Add(poly[k]);
                AppendCellFan(verts, tris, rev, y);
            }
            else
                AppendCellFan(verts, tris, poly, y);
        }

        private static float CuttedMeshSignedAreaXz(List<Vector2> poly)
        {
            double s = 0;
            int n = poly.Count;
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                s += (double)poly[i].x * poly[j].y - (double)poly[j].x * poly[i].y;
            }

            return (float)(0.5 * s);
        }

        private static void AppendCellFan(List<Vector3> verts, List<int> tris, List<Vector2> poly, float y)
        {
            int n = poly.Count;
            Vector3 hub = Vector3.zero;
            var rim = new Vector3[n];
            for (int k = 0; k < n; k++)
            {
                Vector2 p = poly[k];
                rim[k] = new Vector3(p.x, y, p.y);
                hub += rim[k];
            }

            hub /= n;

            int start = verts.Count;
            verts.Add(hub);
            for (int k = 0; k < n; k++)
                verts.Add(rim[k]);

            for (int k = 0; k < n; k++)
            {
                tris.Add(start);
                tris.Add(start + 1 + k);
                tris.Add(start + 1 + ((k + 1) % n));
            }
        }

#if UNITY_EDITOR
        /// <summary>Editor: spawn <see cref="CreateCuttedMeshFromVoronator"/> only (polygon fill).</summary>
        public static GameObject SpawnCuttedPolygonMeshInScene(Voronator v, string objectName = "VoronoiCut")
        {
            Mesh fill = CreateCuttedMeshFromVoronator(v);
            if (fill.vertexCount == 0)
                return null;

            var go = new GameObject(objectName);
            UnityEditor.Undo.RegisterCreatedObjectUndo(go, objectName);
            var mf = go.AddComponent<MeshFilter>();
            var mr = go.AddComponent<MeshRenderer>();
            mf.sharedMesh = fill;

            Shader shader = Shader.Find("Universal Render Pipeline/Lit")
                ?? Shader.Find("HDRP/Lit")
                ?? Shader.Find("Standard")
                ?? Shader.Find("Unlit/Color");
            if (shader == null)
            {
                Debug.LogError("SpawnCuttedPolygonMeshInScene: no compatible fill shader found.");
                UnityEngine.Object.DestroyImmediate(go);
                return null;
            }
            mr.sharedMaterial = new Material(shader);

            if (UnityEditor.Selection.activeTransform != null)
                go.transform.SetPositionAndRotation(
                    UnityEditor.Selection.activeTransform.position,
                    UnityEditor.Selection.activeTransform.rotation);
            else
                go.transform.position = Vector3.zero;

            UnityEditor.Selection.activeGameObject = go;
            return go;
        }

        /// <summary>Editor: clipped Voronoi fill only (no lines). Pair with <see cref="SpawnVoronoiInternalBorderLinesMeshInScene"/>.</summary>
        public static GameObject CreateCuttedMeshAndSpawnInScene(int siteCount, float size, int seed, string objectName = "VoronoiCutted")
        {
            var v = VoronatorFromParams.Build(siteCount, size, seed);
            return SpawnCuttedPolygonMeshInScene(v, objectName);
        }

        /// <summary>Editor: second step — <see cref="VoronatorFromParams.Build"/> + line mesh parented under <paramref name="parent"/> (same siteCount/size/seed as cut).</summary>
        public static GameObject SpawnVoronoiInternalBorderLinesMeshInScene(
            int siteCount,
            float size,
            int seed,
            Transform parent,
            string objectName = "VoronoiInternalBorders")
        {
            var v = VoronatorFromParams.Build(siteCount, size, seed);
            return SpawnVoronoiInternalBorderLinesMeshAsChild(v, parent, objectName);
        }

        /// <summary>Editor: <see cref="CreateVoronoiInternalBorderLinesMesh"/> as child of <paramref name="parent"/> (lines layer after cut).</summary>
        public static GameObject SpawnVoronoiInternalBorderLinesMeshAsChild(
            Voronator v,
            Transform parent,
            string objectName = "VoronoiInternalBorders")
        {
            Mesh lineMesh = CreateVoronoiInternalBorderLinesMesh(v);
            if (lineMesh.vertexCount == 0)
                return null;

            var child = new GameObject(objectName);
            UnityEditor.Undo.RegisterCreatedObjectUndo(child, objectName);
            child.transform.SetParent(parent, false);

            var mf = child.AddComponent<MeshFilter>();
            var mr = child.AddComponent<MeshRenderer>();
            mf.sharedMesh = lineMesh;

            Shader lineShader = Shader.Find("Universal Render Pipeline/Unlit")
                ?? Shader.Find("Unlit/Color")
                ?? Shader.Find("Sprites/Default")
                ?? Shader.Find("Universal Render Pipeline/Lit");
            if (lineShader == null)
            {
                Debug.LogError("SpawnVoronoiInternalBorderLinesMeshAsChild: no compatible line shader found.");
                UnityEngine.Object.DestroyImmediate(child);
                return null;
            }
            var mat = new Material(lineShader);
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", Color.black);
            else if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", Color.black);
            mr.sharedMaterial = mat;

            return child;
        }

        /// <summary>Editor: line mesh only, child of <paramref name="parent"/> (same transform space as fill).</summary>
        public static GameObject CreateVoronoiInternalBorderLinesSpawnInScene(
            Voronator v,
            Transform parent,
            string objectName = "VoronoiInternalBorders")
        {
            Mesh lineMesh = CreateVoronoiInternalBorderLinesMesh(v);
            if (lineMesh.vertexCount == 0)
                return null;

            var child = new GameObject(objectName);
            UnityEditor.Undo.RegisterCreatedObjectUndo(child, objectName);
            child.transform.SetParent(parent, false);

            var mf = child.AddComponent<MeshFilter>();
            var mr = child.AddComponent<MeshRenderer>();
            mf.sharedMesh = lineMesh;

            Shader lineShader = Shader.Find("Universal Render Pipeline/Unlit")
                ?? Shader.Find("Unlit/Color")
                ?? Shader.Find("Sprites/Default")
                ?? Shader.Find("Universal Render Pipeline/Lit");
            if (lineShader == null)
            {
                Debug.LogError("CreateVoronoiInternalBorderLinesSpawnInScene: no compatible line shader found.");
                UnityEngine.Object.DestroyImmediate(child);
                return null;
            }
            var mat = new Material(lineShader);
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", Color.black);
            else if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", Color.black);
            mr.sharedMaterial = mat;

            return child;
        }
#endif
    }
}
