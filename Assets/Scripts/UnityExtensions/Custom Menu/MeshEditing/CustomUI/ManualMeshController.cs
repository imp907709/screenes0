using Meshes.GeneralMesh;
using Meshes.ManualMesh;
using System.Collections.Generic;
using UnityEngine;

namespace UnityExtensions.Custom_Menu.MeshEditing.CustomUI
{
    public class ManualMeshController
    {
        /// <summary>
        /// Last mesh created by <see cref="GO"/> (or assign with <see cref="BindPlaneMeshObject"/>).
        /// Use this when calling <see cref="ApplyAddNoiseToExistingMesh"/> from sliders so edits hit the same object.
        /// </summary>
        public static GameObject ActivePlaneMeshObject { get; private set; }

        /// <summary>Point noise updates at any scene object that has a <see cref="MeshFilter"/> with a mesh.</summary>
        public static void BindPlaneMeshObject(GameObject go) => ActivePlaneMeshObject = go;

        public static void GO(float amp, float freq)
        {
            // var mesh = ManualMesh.CreateOctahedron(radius);
            // var mesh = ManualMesh.CreateHexagon(radius);
            // var mesh = ManualMesh.CreateAngled(angles);
            // var mesh = ManualMesh.CreatePlane();
            // var mesh = ManualMesh.CreatePlaneAdjusted();
            // GameObject go = MeshExamples.CreateHexGridMeshObject();
            
            GameObject go = ManualMeshChecks.CreatePlaneMeshObject(amp,freq);
            go.transform.position = Vector3.zero;
            ActivePlaneMeshObject = go;
        }

        /// <summary>
        /// Reads verts from <see cref="MeshFilter.sharedMesh"/>, runs <see cref="MeshBlob.AddNoise"/> (y += Perlin·amp), writes back.
        /// If <paramref name="resetYBeforeNoise"/> is true (default), Y is cleared first so repeated calls do not stack; same net result as noise on a flat plane.
        /// Set false to truly accumulate each call (y += … again).
        /// </summary>
        public static bool ApplyAddNoiseToExistingMesh(
            GameObject go,
            float amplitude,
            float frequency,
            bool resetYBeforeNoise = true)
        {
            if (go == null)
                return false;

            var mf = go.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null)
                return false;

            Mesh mesh = mf.sharedMesh;
            var verts = new List<Vector3>();
            mesh.GetVertices(verts);
            if (verts.Count == 0)
                return false;

            if (resetYBeforeNoise)
            {
                for (int i = 0; i < verts.Count; i++)
                {
                    Vector3 v = verts[i];
                    v.y = 0f;
                    verts[i] = v;
                }
            }

            MeshBlob.AddNoise(verts, amplitude, frequency);

            mesh.SetVertices(verts);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return true;
        }
    }
}