using System.Collections.Generic;
using Meshes.Voronoi.VoronatorSharp;
using UnityEngine;

namespace Meshes.Voronoi
{
    /// <summary>
    /// Splits a mesh along Voronoi cell boundaries in XZ (each input triangle is clipped to each clipped Voronoi polygon).
    /// Preserves per-vertex height by barycentric interpolation on the original triangle in XZ.
    /// </summary>
    public static class VoronoiPlanarMeshCut
    {
        private const float Eps = 1e-5f;

        /// <summary>
        /// True when the mesh AABB is a thin slab in Y compared to its XZ footprint (horizontal plane / terrain).
        /// Cubes and other volumetric meshes must not use XZ-only clipping — side faces degenerate in projection.
        /// </summary>
        public static bool IsMeshEligibleForXZSlabCut(Bounds bounds, float maxThicknessToFootprintRatio = 0.12f)
        {
            float hx = bounds.size.x;
            float hz = bounds.size.z;
            float hy = Mathf.Max(bounds.size.y, 1e-9f);
            float footprint = Mathf.Max(hx, hz);
            if (footprint < 1e-5f)
                return false;
            return hy / footprint <= maxThicknessToFootprintRatio;
        }

        public static void CutMeshWithVoronoi(Mesh mesh, Voronator voronator, int siteCount)
        {
            var inVerts = new List<Vector3>();
            mesh.GetVertices(inVerts);
            var inTris = new List<int>();
            mesh.GetTriangles(inTris, 0);

            var cellPolys = new List<List<Vector2>>(siteCount);
            for (int i = 0; i < siteCount; i++)
            {
                var poly = voronator.GetClippedPolygon(i);
                if (poly == null || poly.Count < 3)
                {
                    cellPolys.Add(null);
                    continue;
                }

                EnsureCounterClockwise(poly);
                cellPolys.Add(poly);
            }

            var outVerts = new List<Vector3>();
            var outTris = new List<int>();

            int triCount = inTris.Count / 3;
            for (int ti = 0; ti < triCount; ti++)
            {
                int i0 = inTris[ti * 3];
                int i1 = inTris[ti * 3 + 1];
                int i2 = inTris[ti * 3 + 2];
                Vector3 v0 = inVerts[i0];
                Vector3 v1 = inVerts[i1];
                Vector3 v2 = inVerts[i2];

                var triXz = new List<Vector2>(3)
                {
                    new Vector2(v0.x, v0.z),
                    new Vector2(v1.x, v1.z),
                    new Vector2(v2.x, v2.z)
                };

                if (Mathf.Abs(PolygonSignedArea(triXz)) < 1e-10f)
                    continue;

                for (int ci = 0; ci < cellPolys.Count; ci++)
                {
                    var cell = cellPolys[ci];
                    if (cell == null)
                        continue;

                    var subject = new List<Vector2>(triXz);
                    var clipped = ClipPolygonToConvexPolygon(subject, cell);
                    if (clipped == null || clipped.Count < 3)
                        continue;

                    AppendFan(clipped, v0, v1, v2, outVerts, outTris);
                }
            }

            mesh.Clear();
            mesh.SetVertices(outVerts);
            mesh.SetTriangles(outTris, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

        private static void AppendFan(
            List<Vector2> polyXz,
            Vector3 v0,
            Vector3 v1,
            Vector3 v2,
            List<Vector3> outVerts,
            List<int> outTris)
        {
            int start = outVerts.Count;
            for (int i = 0; i < polyXz.Count; i++)
            {
                Vector2 p = polyXz[i];
                Vector3 bc = BarycentricFromTriangleXZ(v0, v1, v2, p);
                float y = bc.x * v0.y + bc.y * v1.y + bc.z * v2.y;
                outVerts.Add(new Vector3(p.x, y, p.y));
            }

            for (int i = 1; i < polyXz.Count - 1; i++)
            {
                outTris.Add(start);
                outTris.Add(start + i);
                outTris.Add(start + i + 1);
            }
        }

        private static Vector3 BarycentricFromTriangleXZ(Vector3 a, Vector3 b, Vector3 c, Vector2 p)
        {
            Vector2 a2 = new Vector2(a.x, a.z);
            Vector2 b2 = new Vector2(b.x, b.z);
            Vector2 c2 = new Vector2(c.x, c.z);
            Vector2 v0 = b2 - a2;
            Vector2 v1 = c2 - a2;
            Vector2 v2 = p - a2;
            float d00 = Vector2.Dot(v0, v0);
            float d01 = Vector2.Dot(v0, v1);
            float d11 = Vector2.Dot(v1, v1);
            float d20 = Vector2.Dot(v2, v0);
            float d21 = Vector2.Dot(v2, v1);
            float denom = d00 * d11 - d01 * d01;
            if (Mathf.Abs(denom) < 1e-14f)
                return new Vector3(1f / 3f, 1f / 3f, 1f / 3f);

            float bv = (d11 * d20 - d01 * d21) / denom;
            float bw = (d00 * d21 - d01 * d20) / denom;
            float bu = 1f - bv - bw;
            return new Vector3(bu, bv, bw);
        }

        private static float PolygonSignedArea(List<Vector2> p)
        {
            double sum = 0.0;
            int n = p.Count;
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                sum += (double)p[i].x * p[j].y - (double)p[j].x * p[i].y;
            }

            return (float)(0.5 * sum);
        }

        private static void EnsureCounterClockwise(List<Vector2> poly)
        {
            if (PolygonSignedArea(poly) < 0f)
                poly.Reverse();
        }

        /// <summary>Subject polygon clipped to lie inside convex CCW clip polygon.</summary>
        private static List<Vector2> ClipPolygonToConvexPolygon(List<Vector2> subject, List<Vector2> clipConvexCcw)
        {
            List<Vector2> output = subject;
            int n = clipConvexCcw.Count;
            for (int i = 0; i < n; i++)
            {
                Vector2 a = clipConvexCcw[i];
                Vector2 b = clipConvexCcw[(i + 1) % n];
                output = ClipPolygonByHalfPlane(output, a, b);
                if (output.Count < 3)
                    return output;
            }

            return output;
        }

        /// <summary>Keep points on the left of directed edge A→B (interior of a CCW convex polygon).</summary>
        private static List<Vector2> ClipPolygonByHalfPlane(List<Vector2> input, Vector2 a, Vector2 b)
        {
            var output = new List<Vector2>();
            int cnt = input.Count;
            if (cnt == 0)
                return output;

            for (int i = 0; i < cnt; i++)
            {
                Vector2 curr = input[i];
                Vector2 prev = input[(i - 1 + cnt) % cnt];
                bool currIn = IsLeftOfEdge(a, b, curr);
                bool prevIn = IsLeftOfEdge(a, b, prev);
                if (currIn)
                {
                    if (!prevIn && SegmentIntersectInfiniteLine(prev, curr, a, b, out Vector2 hit))
                        output.Add(hit);
                    output.Add(curr);
                }
                else if (prevIn && SegmentIntersectInfiniteLine(prev, curr, a, b, out Vector2 hit2))
                {
                    output.Add(hit2);
                }
            }

            return output;
        }

        private static bool IsLeftOfEdge(Vector2 a, Vector2 b, Vector2 p)
        {
            float cross = (b.x - a.x) * (p.y - a.y) - (b.y - a.y) * (p.x - a.x);
            return cross >= -Eps;
        }

        private static bool SegmentIntersectInfiniteLine(
            Vector2 segA,
            Vector2 segB,
            Vector2 lineA,
            Vector2 lineB,
            out Vector2 hit)
        {
            hit = default;
            Vector2 r = segB - segA;
            Vector2 s = lineB - lineA;
            float rxs = Cross2(r, s);
            if (Mathf.Abs(rxs) < 1e-12f)
                return false;

            Vector2 qp = lineA - segA;
            float t = Cross2(qp, s) / rxs;
            if (t < -Eps || t > 1f + Eps)
                return false;

            hit = segA + t * r;
            return true;
        }

        private static float Cross2(Vector2 u, Vector2 v) => u.x * v.y - u.y * v.x;
    }
}
