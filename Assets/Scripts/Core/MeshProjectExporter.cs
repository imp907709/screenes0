using System.IO;
using Init;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core
{
    /// <summary>
    /// Persists a runtime <see cref="Mesh"/> as a Unity mesh asset under <see cref="PathConstants.MeshAssetFolder"/> (Editor only).
    /// </summary>
    public static class MeshProjectExporter
    {
        public static void SaveMeshAsAsset(Mesh sourceMesh, string fileNameWithoutExtension = null)
        {
#if UNITY_EDITOR
            if (sourceMesh == null)
            {
                Debug.LogWarning("SaveMeshAsAsset: mesh is null.");
                return;
            }

            string rawName = string.IsNullOrWhiteSpace(fileNameWithoutExtension)
                ? sourceMesh.name
                : fileNameWithoutExtension;
            string baseName = SafeAssetBaseName(rawName);

            string unityPath = PathConstants.MeshAssetFolder + "/" + baseName + PathConstants.MeshAssetExtension;

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
            meshCopy.name = baseName;
            AssetDatabase.CreateAsset(meshCopy, unityPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Mesh asset saved: " + unityPath);
#else
        Debug.LogWarning("SaveMeshAsAsset works only in the Unity Editor.");
#endif
        }

        /// <summary>Strip path/extension, replace illegal file-name characters (mesh names often include e.g. ':').</summary>
        private static string SafeAssetBaseName(string raw)
        {
            string s = Path.GetFileNameWithoutExtension((raw ?? "").Trim());
            if (string.IsNullOrEmpty(s))
                return "Mesh";
            foreach (char c in Path.GetInvalidFileNameChars())
                s = s.Replace(c, '_');
            return string.IsNullOrEmpty(s) ? "Mesh" : s;
        }
    }
}
