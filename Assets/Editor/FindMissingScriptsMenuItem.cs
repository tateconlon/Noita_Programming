using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FindMissingScriptsTool
{
    [MenuItem("Tools/Missing Scripts/Find")]
    static void FindMissingScriptsMenuItem()
    {
        foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>(true))
        {
            foreach (Component component in go.GetComponentsInChildren<Component>())
            {
                if (component == null)
                {
                    Debug.Log($"GameObject found with missing script {go.name}", go);
                    break;
                }
            }
        }
    }
}
