using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Persists a runtime <see cref="Mesh"/> as a Unity mesh asset under Assets/ (Editor only).
/// Player builds cannot write project assets; use <see cref="ObjExporter"/> / file export there.
/// </summary>
public static class CubeMeshProjectExporter
{
    /// <summary>
    /// If the path does not start with <c>Assets/</c>, it is treated as relative to the Assets folder
    /// (e.g. <c>GeneratedMeshes/Cube.asset</c> → <c>Assets/GeneratedMeshes/Cube.asset</c>).
    /// </summary>
    public static string ToUnityAssetPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return path;

        path = path.Trim().Replace('\\', '/');
        while (path.StartsWith("./", StringComparison.Ordinal))
            path = path.Substring(2);

        if (path.StartsWith("Assets/", StringComparison.Ordinal))
            return path;

        if (path.StartsWith("/", StringComparison.Ordinal))
            path = path.TrimStart('/');

        return "Assets/" + path;
    }

    public static void SaveMeshAsAsset(Mesh sourceMesh, string assetPath = "GeneratedMeshes/GeneratedCube.asset")
    {
#if UNITY_EDITOR
        if (sourceMesh == null)
        {
            Debug.LogWarning("SaveMeshAsAsset: mesh is null.");
            return;
        }

        string unityPath = ToUnityAssetPath(assetPath);
        if (string.IsNullOrEmpty(unityPath) || !unityPath.StartsWith("Assets/", StringComparison.Ordinal))
        {
            Debug.LogError("SaveMeshAsAsset: invalid asset path: " + assetPath);
            return;
        }

        string relativeToAssets = unityPath.Substring("Assets/".Length).Replace('/', Path.DirectorySeparatorChar);
        string fullPath = Path.Combine(Application.dataPath, relativeToAssets);
        string fullDir = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(fullDir) && !Directory.Exists(fullDir))
        {
            Directory.CreateDirectory(fullDir);
            AssetDatabase.Refresh();
        }

        if (AssetDatabase.LoadAssetAtPath<Object>(unityPath) != null)
            AssetDatabase.DeleteAsset(unityPath);

        var meshCopy = Object.Instantiate(sourceMesh);
        meshCopy.name = Path.GetFileNameWithoutExtension(unityPath);
        AssetDatabase.CreateAsset(meshCopy, unityPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Mesh asset saved: " + unityPath);
#else
        Debug.LogWarning("SaveMeshAsAsset works only in the Unity Editor.");
#endif
    }
}
