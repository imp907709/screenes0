using System.Collections.Generic;
using UnityEngine;

namespace Meshes
{
    /// <summary>
    /// Smallest "filled" mesh: you give a hub point and boundary dots in order around one loop.
    /// Each boundary edge is linked to the hub as a triangle fan — reads as one solid sector / cap.
    /// </summary>
    public static class SimpleFanFillMesh
    {
        /// <param name="hub">Usually the sector apex (pie origin).</param>
        /// <param name="perimeter">Boundary points in order along the outer edge (do not repeat the first at the end). At least 3.</param>
        public static Mesh Create(Vector3 hub, IReadOnlyList<Vector3> perimeter)
        {
            int n = perimeter.Count;
            if (n < 3)
                throw new System.ArgumentException("Need at least 3 perimeter points.");

            var verts = new List<Vector3>(n + 1) { hub };
            for (int i = 0; i < n; i++)
                verts.Add(perimeter[i]);

            var tris = new List<int>(n * 3);
            for (int i = 0; i < n; i++)
            {
                tris.Add(0);
                tris.Add(1 + i);
                tris.Add(1 + ((i + 1) % n));
            }

            var mesh = new Mesh { name = "FanFill" };
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        /// <summary>Example: one XZ sector around <paramref name="apex"/> from angle A to B (degrees), radius.</summary>
        public static Mesh CreateSectorXZ(Vector3 apex, float angleStartDeg, float angleEndDeg, float radius, int arcSegments)
        {
            int seg = Mathf.Max(2, arcSegments);
            var rim = new List<Vector3>(seg + 1);
            float t0 = angleStartDeg * Mathf.Deg2Rad;
            float t1 = angleEndDeg * Mathf.Deg2Rad;
            for (int i = 0; i <= seg; i++)
            {
                float t = Mathf.Lerp(t0, t1, i / (float)seg);
                rim.Add(apex + new Vector3(Mathf.Cos(t) * radius, 0f, Mathf.Sin(t) * radius));
            }

            return Create(apex, rim);
        }
        
        /// <summary>Convex-ish irregular loop in XZ (y=0), CCW from above — reads as one organic cell.</summary>
        public static Vector3[] BuildIrregularCellRim(int count, float scale)
        {
            count = count == 5 ? 5 : 7;
            scale = Mathf.Max(0.01f, scale);
            var pts = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                float baseAngle = (i / (float)count) * Mathf.PI * 2f;
                float r = scale * (0.82f + 0.18f * Mathf.Sin(i * 2.17f + 0.6f));
                float a = baseAngle + 0.11f * Mathf.Sin(i * 1.9f);
                pts[i] = new Vector3(Mathf.Cos(a) * r, 0f, Mathf.Sin(a) * r);
            }

            return pts;
        }

        public static Vector3 Centroid(Vector3[] pts)
        {
            Vector3 s = Vector3.zero;
            for (int i = 0; i < pts.Length; i++)
                s += pts[i];
            return s / Mathf.Max(1, pts.Length);
        }
    }
}
