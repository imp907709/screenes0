using Init;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Binding.UI
{
    public class GenerateButtonBinder : MonoBehaviour
    {
        private IProceduralMeshController _controller;

        public void Init(IProceduralMeshController controller)
        {
            Debug.Log("GenerateButtonBinder inited");

            _controller = controller;

            Bind(UIConstants.GenerateObjButtonName, OnGenerateClick);
            Bind(UIConstants.ReGenerateObjButtonName, OnRegenerateClick);
            Bind(UIConstants.ExportMeshButtonName, OnExportMeshClick);

            Debug.Log("Binder initialized");
        }

        private void Bind(string buttonName, UnityAction onClick)
        {
            var go = GameObject.Find(buttonName);
            if (go == null)
            {
                Debug.Log("Button not found: " + buttonName);
                return;
            }

            var button = go.GetComponent<Button>();
            if (button == null)
            {
                Debug.LogError("No Button component on: " + buttonName);
                return;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(onClick);
        }

        private void OnGenerateClick()
        {
            if (_controller == null)
            {
                Debug.LogError("Controller not injected");
                return;
            }

            _controller.Generate();
        }

        private void OnRegenerateClick()
        {
            if (_controller == null)
            {
                Debug.LogError("Controller not injected");
                return;
            }

            _controller.ReGenerate();
        }

        private void OnExportMeshClick()
        {
            if (_controller == null)
            {
                Debug.LogError("Controller not injected");
                return;
            }

            _controller.ExportMeshAsProjectAsset();
        }
    }
}
