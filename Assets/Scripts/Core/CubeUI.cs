using UnityEngine;

public class CubeUI : MonoBehaviour
{
    public CubeController controller;

    void OnGUI()
    {
        if (GUILayout.Button("Generate Cube"))
        {
            controller.Generate();
        }

        if (GUILayout.Button("Export Cube"))
        {
            controller.Export();
        }
    }
}