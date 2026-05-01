using Core;
using UnityEngine;
using UnityEngine.UI;

public class GenerateButtonBinder : MonoBehaviour
{
    private Button _button;
    private CubeController _controller;

    public void Init(CubeController controller)
    {
        Debug.Log("GenerateButtonBinder inited");
        
        _controller = controller;

        var go = GameObject.Find(UIConstants.GenerateObjButtonName);

        if (go == null)
        {
            Debug.Log("Button not found: " + UIConstants.GenerateObjButtonName);
            return;
        }

        _button = go.GetComponent<Button>();

        if (_button == null)
        {
            Debug.LogError("No Button component on: " + UIConstants.GenerateObjButtonName);
            return;
        }

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnClick);

        Debug.Log("Binder initialized");
    }

    private void OnClick()
    {
        if (_controller == null)
        {
            Debug.LogError("Controller not injected");
            return;
        }

        _controller.Generate();
    }
}