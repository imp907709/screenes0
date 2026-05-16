using System.Collections.Generic;
using Meshes.ManualMesh;
using UnityEngine;

namespace UnityExtensions.Custom_Menu.CustomUI.Controllers
{
    public class MeshController
    {
        /// <summary>Last saved mesh vertices (after Generate or &quot;Save noise layer&quot;). UI preview = this + one <see cref="MeshBlob.AddNoise"/>.</summary>
        static List<Vector3> _noiseCommittedVerts;

        /// <summary>After last successful draft preview — used to apply only amplitude delta without re-adding full Perlin when frequency is unchanged.</summary>
        static bool _haveLastDraftNoiseParams;
        static float _lastDraftNoiseAmp;
        static float _lastDraftNoiseFreq;

        /// <summary>
        /// Last mesh created by <see cref="GO"/> (or assign with <see cref="BindPlaneMeshObject"/>).
        /// Use this when calling <see cref="ApplyAddNoiseToExistingMesh"/> from sliders so edits hit the same object.
        /// </summary>
        public static GameObject ActivePlaneMeshObject { get; private set; }

        /// <summary>Point noise updates at any scene object that has a <see cref="MeshFilter"/> with a mesh.</summary>
        public static void BindPlaneMeshObject(GameObject go) => ActivePlaneMeshObject = go;

        public static void GO(float amplitude, float frequency)
        {
            // var mesh = ManualMesh.CreateOctahedron(radius);
            // var mesh = ManualMesh.Createhttps://www.youtube.com/watch?v=s-Gr-KN3ldAHexagon(radius);
            // var mesh = ManualMesh.CreateAngled(angles);
            // var mesh = ManualMesh.CreatePlane();
            // var mesh = ManualMesh.CreatePlaneAdjusted();
            // GameObject go = MeshExamples.CreateHexGridMeshObject();

            GameObject go = ManualMeshChecks.CreatePlaneMeshObject(amplitude, frequency);
            go.transform.position = Vector3.zero;
            ActivePlaneMeshObject = go;
            CaptureCommittedVerticesFromMesh(go);
            ResetDraftNoiseTracking();
        }

        /// <summary>Clears draft delta tracking (call after <see cref="GO"/> so the first UI refresh does a full recompute from committed).</summary>
        public static void ResetDraftNoiseTracking()
        {
            _haveLastDraftNoiseParams = false;
        }

        /// <summary>Call after <see cref="SaveCommittedNoiseLayerFromActiveMesh"/> with current UI draft amp/freq so the next amplitude tweak uses delta against these values.</summary>
        public static void SetDraftNoiseTrackingFromUi(float amplitude, float frequency)
        {
            _lastDraftNoiseAmp = amplitude;
            _lastDraftNoiseFreq = frequency;
            _haveLastDraftNoiseParams = true;
        }

        static void CaptureCommittedVerticesFromMesh(GameObject go)
        {
            if (go == null)
                return;
            var mf = go.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null)
                return;
            var verts = new List<Vector3>();
            mf.sharedMesh.GetVertices(verts);
            _noiseCommittedVerts = verts.Count == 0 ? null : new List<Vector3>(verts);
        }

        /// <summary>
        /// UI live preview: one draft noise layer on top of <see cref="_noiseCommittedVerts"/>.
        /// If frequency matches the last preview, only amplitude delta is applied to current Y (no y=0, no full re-add of the same Perlin term).
        /// If frequency changed (or no prior preview), recomputes from committed + one <see cref="MeshBlob.AddNoise"/> (same as MeshBlob sampling).
        /// </summary>
        public static bool UpdateExistingMeshDraftSingleNoise(GameObject go, float amplitude, float frequency)
        {
            if (go == null)
                return false;

            var mf = go.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null)
                return false;

            UnityEngine.Mesh mesh = mf.sharedMesh;
            if (_noiseCommittedVerts == null || _noiseCommittedVerts.Count != mesh.vertexCount)
                CaptureCommittedVerticesFromMesh(go);

            if (_noiseCommittedVerts == null || _noiseCommittedVerts.Count != mesh.vertexCount)
                return false;

            List<Vector3> verts;

            if (_haveLastDraftNoiseParams && Mathf.Approximately(frequency, _lastDraftNoiseFreq))
            {
                verts = new List<Vector3>();
                mesh.GetVertices(verts);
                if (verts.Count != mesh.vertexCount)
                    return false;

                float deltaAmp = amplitude - _lastDraftNoiseAmp;
                MeshBlob.AddNoise(verts, deltaAmp, frequency);
            }
            else
            {
                verts = new List<Vector3>(_noiseCommittedVerts);
                MeshBlob.AddNoise(verts, amplitude, frequency);
            }

            mesh = MeshGeneral.MeshApply(mesh, verts);

            _lastDraftNoiseAmp = amplitude;
            _lastDraftNoiseFreq = frequency;
            _haveLastDraftNoiseParams = true;
            return true;
        }

        /// <summary>Bakes current mesh (preview) as the new committed base for the next draft noise layer.</summary>
        public static void SaveCommittedNoiseLayerFromActiveMesh()
        {
            if (ActivePlaneMeshObject == null)
                return;
            CaptureCommittedVerticesFromMesh(ActivePlaneMeshObject);
        }
    }
}