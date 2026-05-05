using System.Globalization;
using System.Collections.Generic;
using Meshes.Voronoi.FortunesVoronoi;
using VoronoiLib;
using VoronoiLib.Structures;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityExtensions.Custom_Menu.MeshEditing
{
    public static class VoronoiNewUI
    {
        private static float _size = 10f;
        private static int _siteCount = 24;
        private static int _seed = 1;
        private static float _lineY = 0.01f;

        public static void AddVoronoiNew(VisualElement rootVisualElement)
        {
            var sizeField = new TextField("Fortune Size");
            sizeField.value = _size.ToString(CultureInfo.InvariantCulture);
            sizeField.RegisterValueChangedCallback(evt =>
            {
                if (float.TryParse(evt.newValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed))
                    _size = Mathf.Max(0.01f, parsed);
            });

            var countField = new TextField("Fortune Sites");
            countField.value = _siteCount.ToString();
            countField.RegisterValueChangedCallback(evt =>
            {
                if (int.TryParse(evt.newValue, out int parsed))
                    _siteCount = Mathf.Max(3, parsed);
            });

            var seedField = new TextField("Fortune Seed");
            seedField.value = _seed.ToString();
            seedField.RegisterValueChangedCallback(evt =>
            {
                if (int.TryParse(evt.newValue, out int parsed))
                    _seed = parsed;
            });

            var lineYField = new TextField("Fortune Line Y");
            lineYField.value = _lineY.ToString(CultureInfo.InvariantCulture);
            lineYField.RegisterValueChangedCallback(evt =>
            {
                if (float.TryParse(evt.newValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed))
                    _lineY = parsed;
            });

            var button = MenuCreation._buttonCreate("Generate Voronoi (Fortune)", () =>
            {
                try
                {
                    var root = new GameObject("VoronoiFortuneNew");
                    Undo.RegisterCreatedObjectUndo(root, "Create Voronoi Fortune");
                    if (Selection.activeTransform != null)
                        root.transform.SetPositionAndRotation(Selection.activeTransform.position, Selection.activeTransform.rotation);
                    else
                        root.transform.position = Vector3.zero;

                    var sites = new List<FortuneSite>();
                    VoronoiDemoNew.BuildSites(Mathf.Max(3, _siteCount), Mathf.Max(0.01f, _size), _seed, sites);
                    var edges = FortunesAlgorithm.Run(sites, 0d, 0d, _size, _size);

                    CreateChildMeshObject(
                        root.transform,
                        "VoronoiFortuneCells",
                        VoronoiDemoNew.BuildCellMeshFromEdges(sites, edges, 0f, "FortunesVoronoiCells"),
                        new Color(0.7f, 0.7f, 0.7f, 1f),
                        true);

                    CreateChildMeshObject(
                        root.transform,
                        "VoronoiFortuneLines",
                        VoronoiDemoNew.BuildLineMesh(edges, _lineY, "FortunesVoronoiLines"),
                        Color.black,
                        false);

                    Selection.activeGameObject = root;
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
            });

            rootVisualElement.Add(sizeField);
            rootVisualElement.Add(countField);
            rootVisualElement.Add(seedField);
            rootVisualElement.Add(lineYField);
            rootVisualElement.Add(button);
        }

        private static void CreateChildMeshObject(
            Transform parent,
            string childName,
            Mesh mesh,
            Color color,
            bool preferLit)
        {
            if (mesh == null || mesh.vertexCount == 0)
            {
                Debug.LogError($"VoronoiNewUI: generated mesh '{childName}' is empty.");
                return;
            }

            var child = new GameObject(childName);
            Undo.RegisterCreatedObjectUndo(child, $"Create {childName}");
            child.transform.SetParent(parent, false);

            var mf = child.AddComponent<MeshFilter>();
            var mr = child.AddComponent<MeshRenderer>();
            mf.sharedMesh = mesh;

            Shader shader = preferLit
                ? (Shader.Find("Universal Render Pipeline/Lit")
                    ?? Shader.Find("HDRP/Lit")
                    ?? Shader.Find("Standard")
                    ?? Shader.Find("Unlit/Color"))
                : (Shader.Find("Universal Render Pipeline/Unlit")
                    ?? Shader.Find("Unlit/Color")
                    ?? Shader.Find("Sprites/Default")
                    ?? Shader.Find("Universal Render Pipeline/Lit"));

            if (shader == null)
            {
                Debug.LogError($"VoronoiNewUI: no compatible shader found for '{childName}'.");
                Object.DestroyImmediate(child);
                return;
            }

            var mat = new Material(shader);
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", color);
            else if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", color);
            mr.sharedMaterial = mat;
        }
    }
}
