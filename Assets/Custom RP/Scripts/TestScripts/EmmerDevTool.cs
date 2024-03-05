using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class EmmerDevTool : EditorWindow
{

    const int ButtonHeight = 30;

    public static string CurrentScenePath => SceneManager.GetActiveScene().path;

    [MenuItem("Emmer/Dev Tool")]
    public static void ShowWindow()
    {
        GetWindow(typeof(EmmerDevTool), false, "Emmer Dev Tool");
    }
    //
    private void OnGUI()
    {
        if (GUILayout.Button("Test", GUILayout.Height(ButtonHeight)))
        {
            Debug.Log(CurrentScenePath);
        }
    }

    public enum Test
    {
        Undefined = 0
    }

    

}
