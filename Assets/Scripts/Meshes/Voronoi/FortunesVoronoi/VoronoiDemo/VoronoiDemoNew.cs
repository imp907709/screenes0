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
        public bool overwriteMaterialOnGenerate = true;
        public bool generateOnStart = true;
        public bool regenerateOnValidate = false;
        public string meshName = "FortunesVoronoiLines";

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

            if (mr.sharedMaterial == null && !overwriteMaterialOnGenerate)
            {
                Debug.Log("VoronoiDemoNew: keeping existing sharedMaterial (overwriteMaterialOnGenerate=false).");
                return;
            }

            Shader shader = Shader.Find("Universal Render Pipeline/Unlit")
                ?? Shader.Find("Unlit/Color")
                ?? Shader.Find("Sprites/Default")
                ?? Shader.Find("Standard");
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

        private static string BuildUndirectedEdgeKey(Vector3 a, Vector3 b)
        {
            string sa = a.x.ToString("G9", CultureInfo.InvariantCulture) + "," +
                        a.z.ToString("G9", CultureInfo.InvariantCulture);
            string sb = b.x.ToString("G9", CultureInfo.InvariantCulture) + "," +
                        b.z.ToString("G9", CultureInfo.InvariantCulture);
            return string.CompareOrdinal(sa, sb) <= 0 ? sa + ";" + sb : sb + ";" + sa;
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
