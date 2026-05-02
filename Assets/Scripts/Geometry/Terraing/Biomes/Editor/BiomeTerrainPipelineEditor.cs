#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BiomeTerrainPipeline))]
public class BiomeTerrainPipelineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var pipe = (BiomeTerrainPipeline)target;
        var mf = pipe.GetMeshFilter();

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Biome terrain (one mesh)", EditorStyles.boldLabel);

        if (GUILayout.Button("Step 1 — Continental shape (macro)", GUILayout.Height(28)))
        {
            Undo.RecordObject(pipe, "Biome Step 1 Continental");
            if (mf != null)
                Undo.RecordObject(mf, "Biome Step 1 Continental");

            Mesh created = mf != null ? pipe.RebuildMeshFromGrid() : null;
            if (created != null)
                Undo.RegisterCompleteObjectUndo(created, "Biome continental mesh");

            pipe.ApplyContinentalToWorkingMesh();

            if (mf != null && mf.sharedMesh != null)
                EditorUtility.SetDirty(mf.sharedMesh);

            EditorUtility.SetDirty(pipe);
        }
    }
}
#endif
