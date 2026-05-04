using System.Linq;
using Init;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Binding.UI
{
    /// <summary>
    /// Binds a <see cref="TMP_Dropdown"/> or legacy <see cref="Dropdown"/> named <see cref="UIConstants.GeometrySelectorName"/>
    /// to <see cref="Init.MeshShapeRegistry"/> and applies selection to <see cref="IProceduralMeshController"/>.
    /// </summary>
    public class GeometrySelectorBinder : MonoBehaviour
    {
        private IProceduralMeshController _controller;
        private TMP_Dropdown _tmpDropdown;
        private Dropdown _legacyDropdown;

        public void Init(IProceduralMeshController controller, string initialShapeId)
        {
            _controller = controller;

            var go = GameObject.Find(UIConstants.GeometrySelectorName);
            if (go == null)
            {
                Debug.Log("Geometry selector not found: " + UIConstants.GeometrySelectorName);
                return;
            }

            _tmpDropdown = go.GetComponent<TMP_Dropdown>();
            if (_tmpDropdown != null)
            {
                SetupTmp(initialShapeId);
                return;
            }

            _legacyDropdown = go.GetComponent<Dropdown>();
            if (_legacyDropdown != null)
            {
                SetupLegacy(initialShapeId);
                return;
            }

            Debug.LogError("No TMP_Dropdown or Dropdown on: " + UIConstants.GeometrySelectorName);
        }

        private void SetupTmp(string initialShapeId)
        {
            var options = MeshShapeRegistry.All.Select(o => new TMP_Dropdown.OptionData(o.DisplayName)).ToList();
            _tmpDropdown.ClearOptions();
            _tmpDropdown.AddOptions(options);
            _tmpDropdown.onValueChanged.RemoveAllListeners();
            _tmpDropdown.onValueChanged.AddListener(OnIndexChanged);

            int idx = MeshShapeRegistry.IndexOfId(initialShapeId);
            if (idx < 0) idx = 0;
            _tmpDropdown.SetValueWithoutNotify(idx);
        }

        private void SetupLegacy(string initialShapeId)
        {
            _legacyDropdown.ClearOptions();
            _legacyDropdown.AddOptions(MeshShapeRegistry.All.Select(o => o.DisplayName).ToList());
            _legacyDropdown.onValueChanged.RemoveAllListeners();
            _legacyDropdown.onValueChanged.AddListener(OnIndexChanged);

            int idx = MeshShapeRegistry.IndexOfId(initialShapeId);
            if (idx < 0) idx = 0;
            _legacyDropdown.SetValueWithoutNotify(idx);
        }

        private void OnIndexChanged(int index)
        {
            if (_controller == null)
            {
                Debug.LogError("Geometry selector: controller not set");
                return;
            }

            if (index < 0 || index >= MeshShapeRegistry.All.Count) return;

            string id = MeshShapeRegistry.All[index].Id;
            _controller.SetShapeById(id);
            // _controller.ReGenerate();
        }
    }
}
