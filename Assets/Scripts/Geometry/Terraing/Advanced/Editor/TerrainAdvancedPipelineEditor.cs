#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainAdvancedPipeline))]
public class TerrainAdvancedPipelineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var pipe = (TerrainAdvancedPipeline)target;
        var mf = pipe.GetMeshFilter();

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Pipeline (same mesh)", EditorStyles.boldLabel);

        if (GUILayout.Button("Step 1 — Fractal noise (fBm)", GUILayout.Height(28)))
        {
            Undo.RecordObject(pipe, "Terrain Step 1");
            if (mf != null)
                Undo.RecordObject(mf, "Terrain Step 1");

            Mesh created = null;
            if (mf != null)
                created = pipe.RebuildMeshFromGrid();

            if (created != null)
                Undo.RegisterCompleteObjectUndo(created, "Terrain Step 1 mesh");

            pipe.ApplyFractalNoiseToWorkingMesh();

            if (mf != null && mf.sharedMesh != null)
                EditorUtility.SetDirty(mf.sharedMesh);

            EditorUtility.SetDirty(pipe);
        }

        EditorGUILayout.HelpBox("Hydraulic erosion on current heights (run Step 1 first).", MessageType.None);
        if (GUILayout.Button("Step 2 — Hydraulic erosion", GUILayout.Height(28)))
        {
            if (mf == null || mf.sharedMesh == null)
            {
                Debug.LogWarning("TerrainAdvancedPipeline: MeshFilter or mesh missing.");
            }
            else
            {
                Undo.RecordObject(pipe, "Terrain Step 2");
                Undo.RecordObject(mf.sharedMesh, "Terrain Step 2");
                pipe.ApplyStep2_HydraulicErosion();
                EditorUtility.SetDirty(mf.sharedMesh);
            }

            EditorUtility.SetDirty(pipe);
        }

        EditorGUILayout.HelpBox("Thermal / talus on current heights.", MessageType.None);
        if (GUILayout.Button("Step 3 — Thermal erosion", GUILayout.Height(28)))
        {
            if (mf == null || mf.sharedMesh == null)
            {
                Debug.LogWarning("TerrainAdvancedPipeline: MeshFilter or mesh missing.");
            }
            else
            {
                Undo.RecordObject(pipe, "Terrain Step 3");
                Undo.RecordObject(mf.sharedMesh, "Terrain Step 3");
                pipe.ApplyStep3_ThermalErosion();
                EditorUtility.SetDirty(mf.sharedMesh);
            }

            EditorUtility.SetDirty(pipe);
        }
    }
}
#endif
