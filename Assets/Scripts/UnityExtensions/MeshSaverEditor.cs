using UnityEditor;
using UnityEngine;

public class MeshSaverEditor : Editor {
    [MenuItem("Context/MeshFilter/Save Mesh As Asset")]
    public static void SaveMeshAsset(MenuCommand menuCommand) {
        MeshFilter mf = menuCommand.context as MeshFilter;
        if (mf == null)
            mf = Selection.activeGameObject?.GetComponent<MeshFilter>();

        if (mf == null)
        {
            Debug.LogError("No MeshFilter context found. Make sure you right-click a MeshFilter.");
            return;
        }
        if (mf.sharedMesh == null)
        {
            Debug.LogError("MeshFilter has no mesh assigned.");
            return;
        }
        
        MeshProjectExporter.SaveMeshAsAsset(mf.sharedMesh);
    }
}