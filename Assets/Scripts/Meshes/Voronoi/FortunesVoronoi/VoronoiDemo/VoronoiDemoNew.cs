using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using VoronoiLib;
using VoronoiLib.Structures;

namespace Meshes.Voronoi.FortunesVoronoi
{
    /// <summary>
    /// Unity-facing demo runner for VoronoiLib (Fortune algorithm).
    /// Generates random sites in [0,size]x[0,size] and builds a line mesh from clipped Voronoi edges.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class VoronoiDemoNew : MonoBehaviour
    {
        [Header("Input")]
        [Min(3)] public int siteCount = 24;
        [Min(0.01f)] public float size = 10f;
        public int seed = 1;

        [Header("Output")]
        public float lineY = 0.01f;
        public Color lineColor = Color.black;
        public float cellY = 0f;
        public Color cellColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        public bool overwriteMaterialOnGenerate = true;
        public bool generateOnStart = true;
        public bool regenerateOnValidate = false;
        public string meshName = "FortunesVoronoiLines";
        public string cellMeshName = "FortunesVoronoiCells";

        private readonly List<FortuneSite> _sites = new List<FortuneSite>();
        private LinkedList<VEdge> _edges = new LinkedList<VEdge>();

        public IReadOnlyList<FortuneSite> Sites => _sites;
        public LinkedList<VEdge> Edges => _edges;

        private void Start()
        {
            if (generateOnStart)
                Generate();
        }

        private void OnValidate()
        {
            siteCount = Mathf.Max(3, siteCount);
            size = Mathf.Max(0.01f, size);
            if (regenerateOnValidate && isActiveAndEnabled)
                Generate();
        }

        [ContextMenu("Generate Voronoi")]
        public void Generate()
        {
            BuildSites(siteCount, size, seed, _sites);
            _edges = FortunesAlgorithm.Run(_sites, 0d, 0d, size, size);

            var mesh = BuildLineMesh(_edges, lineY, meshName);
            var mf = GetOrAdd<MeshFilter>(gameObject);
            var mr = GetOrAdd<MeshRenderer>(gameObject);
            mf.sharedMesh = mesh;

            if (!overwriteMaterialOnGenerate && mr.sharedMaterial != null)
            {
                Debug.Log("VoronoiDemoNew: keeping existing sharedMaterial (overwriteMaterialOnGenerate=false).");
                return;
            }

            Shader shader = Shader.Find("Universal Render Pipeline/Unlit")
                ?? Shader.Find("Unlit/Color")
                ?? Shader.Find("Sprites/Default")
                ?? Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                Debug.LogError("VoronoiDemoNew: no compatible shader found for line rendering.");
                return;
            }

            var mat = new Material(shader);
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", lineColor);
            else if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", lineColor);
            mr.sharedMaterial = mat;
        }

        [ContextMenu("Generate Voronoi Cells")]
        public void GenerateCells()
        {
            BuildSites(siteCount, size, seed, _sites);
            _edges = FortunesAlgorithm.Run(_sites, 0d, 0d, size, size);

            var mesh = BuildCellMeshFromEdges(_sites, _edges, cellY, cellMeshName);
            var mf = GetOrAdd<MeshFilter>(gameObject);
            var mr = GetOrAdd<MeshRenderer>(gameObject);
            mf.sharedMesh = mesh;

            if (!overwriteMaterialOnGenerate && mr.sharedMaterial != null)
            {
                Debug.Log("VoronoiDemoNew: keeping existing sharedMaterial (overwriteMaterialOnGenerate=false).");
                return;
            }

            Shader shader = Shader.Find("Universal Render Pipeline/Lit")
                ?? Shader.Find("HDRP/Lit")
                ?? Shader.Find("Standard")
                ?? Shader.Find("Unlit/Color");
            if (shader == null)
            {
                Debug.LogError("VoronoiDemoNew: no compatible shader found for cell rendering.");
                return;
            }

            var mat = new Material(shader);
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", cellColor);
            else if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", cellColor);
            mr.sharedMaterial = mat;
        }

        public static void BuildSites(int count, float size, int seed, List<FortuneSite> output)
        {
            output.Clear();
            var rng = new System.Random(seed);
            var used = new HashSet<string>();
            while (output.Count < count)
            {
                double x = rng.NextDouble() * size;
                double y = rng.NextDouble() * size;
                // Avoid exact duplicate sites (rare, but unstable for Fortune queue ordering).
                string key = x.ToString("G17", CultureInfo.InvariantCulture) + "," +
                             y.ToString("G17", CultureInfo.InvariantCulture);
                if (!used.Add(key))
                    continue;
                output.Add(new FortuneSite(x, y));
            }
        }

        public static Mesh BuildLineMesh(IEnumerable<VEdge> edges, float y, string meshName = "FortunesVoronoiLines")
        {
            var verts = new List<Vector3>();
            var indices = new List<int>();
            var dedup = new HashSet<string>();

            foreach (var edge in edges)
            {
                if (edge?.Start == null || edge.End == null)
                    continue;

                Vector3 a = new Vector3((float)edge.Start.X, y, (float)edge.Start.Y);
                Vector3 b = new Vector3((float)edge.End.X, y, (float)edge.End.Y);
                if ((a - b).sqrMagnitude < 1e-10f)
                    continue;

                string key = BuildUndirectedEdgeKey(a, b);
                if (!dedup.Add(key))
                    continue;

                int i0 = verts.Count;
                verts.Add(a);
                verts.Add(b);
                indices.Add(i0);
                indices.Add(i0 + 1);
            }

            var mesh = new Mesh { name = meshName };
            if (verts.Count == 0)
            {
                mesh.SetVertices(Array.Empty<Vector3>());
                return mesh;
            }

            mesh.SetVertices(verts);
            mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        public static Mesh BuildCellMeshFromSites(IEnumerable<FortuneSite> sites, float y, string meshName = "FortunesVoronoiCells")
        {
            var verts = new List<Vector3>();
            var tris = new List<int>();
            foreach (var site in sites)
            {
                if (site?.Cell == null || site.Cell.Count == 0)
                    continue;

                var poly = ExtractCellPolygon(site);
                if (poly.Count < 3)
                    continue;

                AppendCellFanCcw(verts, tris, poly, y);
            }

            var mesh = new Mesh { name = meshName };
            if (verts.Count == 0)
            {
                mesh.SetVertices(Array.Empty<Vector3>());
                mesh.SetTriangles(Array.Empty<int>(), 0);
                return mesh;
            }

            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        public static Mesh BuildCellMeshFromEdges(
            IEnumerable<FortuneSite> sites,
            IEnumerable<VEdge> edges,
            float y,
            string meshName = "FortunesVoronoiCells")
        {
            var verts = new List<Vector3>();
            var tris = new List<int>();

            var edgeList = new List<VEdge>();
            foreach (var e in edges)
                if (e != null && e.Start != null && e.End != null)
                    edgeList.Add(e);

            foreach (var site in sites)
            {
                if (site == null)
                    continue;

                var poly = ExtractCellPolygonFromEdges(site, edgeList);
                if (poly.Count < 3)
                    continue;

                AppendCellFanCcw(verts, tris, poly, y);
            }

            var mesh = new Mesh { name = meshName };
            if (verts.Count == 0)
            {
                mesh.SetVertices(Array.Empty<Vector3>());
                mesh.SetTriangles(Array.Empty<int>(), 0);
                return mesh;
            }

            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private static string BuildUndirectedEdgeKey(Vector3 a, Vector3 b)
        {
            string sa = a.x.ToString("G9", CultureInfo.InvariantCulture) + "," +
                        a.z.ToString("G9", CultureInfo.InvariantCulture);
            string sb = b.x.ToString("G9", CultureInfo.InvariantCulture) + "," +
                        b.z.ToString("G9", CultureInfo.InvariantCulture);
            return string.CompareOrdinal(sa, sb) <= 0 ? sa + ";" + sb : sb + ";" + sa;
        }

        private static List<Vector2> ExtractCellPolygon(FortuneSite site)
        {
            var unique = new List<Vector2>();
            var seen = new HashSet<string>();
            foreach (var edge in site.Cell)
            {
                if (edge?.Start != null)
                {
                    var p = new Vector2((float)edge.Start.X, (float)edge.Start.Y);
                    string key = p.x.ToString("G9", CultureInfo.InvariantCulture) + "," +
                                 p.y.ToString("G9", CultureInfo.InvariantCulture);
                    if (seen.Add(key))
                        unique.Add(p);
                }

                if (edge?.End != null)
                {
                    var p = new Vector2((float)edge.End.X, (float)edge.End.Y);
                    string key = p.x.ToString("G9", CultureInfo.InvariantCulture) + "," +
                                 p.y.ToString("G9", CultureInfo.InvariantCulture);
                    if (seen.Add(key))
                        unique.Add(p);
                }
            }

            if (unique.Count < 3)
                return unique;

            Vector2 c = Vector2.zero;
            for (int i = 0; i < unique.Count; i++)
                c += unique[i];
            c /= unique.Count;

            unique.Sort((a, b) =>
            {
                float aa = Mathf.Atan2(a.y - c.y, a.x - c.x);
                float bb = Mathf.Atan2(b.y - c.y, b.x - c.x);
                return aa.CompareTo(bb);
            });
            return unique;
        }

        private static List<Vector2> ExtractCellPolygonFromEdges(FortuneSite site, List<VEdge> edges)
        {
            var unique = new List<Vector2>();
            var seen = new HashSet<string>();
            foreach (var edge in edges)
            {
                if (edge.Left != site && edge.Right != site)
                    continue;

                var a = new Vector2((float)edge.Start.X, (float)edge.Start.Y);
                var b = new Vector2((float)edge.End.X, (float)edge.End.Y);
                AddUniquePoint(unique, seen, a);
                AddUniquePoint(unique, seen, b);
            }

            if (unique.Count < 3)
                return unique;

            var center = new Vector2((float)site.X, (float)site.Y);
            unique.Sort((p1, p2) =>
            {
                float a1 = Mathf.Atan2(p1.y - center.y, p1.x - center.x);
                float a2 = Mathf.Atan2(p2.y - center.y, p2.x - center.x);
                return a1.CompareTo(a2);
            });
            return unique;
        }

        private static void AddUniquePoint(List<Vector2> points, HashSet<string> seen, Vector2 p)
        {
            string key = p.x.ToString("G9", CultureInfo.InvariantCulture) + "," +
                         p.y.ToString("G9", CultureInfo.InvariantCulture);
            if (seen.Add(key))
                points.Add(p);
        }

        private static void AppendCellFanCcw(List<Vector3> verts, List<int> tris, List<Vector2> poly, float y)
        {
            if (SignedArea(poly) < 0f)
                poly.Reverse();

            int n = poly.Count;
            Vector3 hub = Vector3.zero;
            var rim = new Vector3[n];
            for (int i = 0; i < n; i++)
            {
                rim[i] = new Vector3(poly[i].x, y, poly[i].y);
                hub += rim[i];
            }
            hub /= n;

            int start = verts.Count;
            verts.Add(hub);
            for (int i = 0; i < n; i++)
                verts.Add(rim[i]);

            for (int i = 0; i < n; i++)
            {
                tris.Add(start);
                tris.Add(start + 1 + i);
                tris.Add(start + 1 + ((i + 1) % n));
            }
        }

        private static float SignedArea(List<Vector2> poly)
        {
            double s = 0d;
            int n = poly.Count;
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                s += (double)poly[i].x * poly[j].y - (double)poly[j].x * poly[i].y;
            }
            return (float)(0.5d * s);
        }

        private static T GetOrAdd<T>(GameObject go) where T : Component
        {
            var c = go.GetComponent<T>();
            if (c == null)
                c = go.AddComponent<T>();
            return c;
        }
    }
}
