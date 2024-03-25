using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public abstract class DataToPrefabGenerator<TPrefabData> : ScriptableObject where TPrefabData : PrefabData
{
    [Required, AssetsOnly]
    [SerializeField] private GameObject _templatePrefab;
    
    [Button]
    public void Run()
    {
        foreach (string guid in AssetDatabase.FindAssets("t:" + typeof(TPrefabData).FullName))
        {
            TPrefabData prefabData = AssetDatabase.LoadAssetAtPath<TPrefabData>(AssetDatabase.GUIDToAssetPath(guid));
            
            if (prefabData == null) continue;

            if (prefabData.prefab == null)
            {
                TryCreatePrefabVariant(_templatePrefab, prefabData.displayName, out prefabData.prefab);
                
                EditorUtility.SetDirty(prefabData);
                AssetDatabase.SaveAssetIfDirty(prefabData);
            }
            
            InjectDataIntoPrefab(prefabData);
            EditorUtility.SetDirty(prefabData.prefab.gameObject);
            AssetDatabase.SaveAssetIfDirty(prefabData.prefab.gameObject);
        }
        
        AssetDatabase.SaveAssets();
    }
    
    private static bool TryCreatePrefabVariant(GameObject originalPrefab, string variantName, out GameObject prefabVariant)
    {
        GameObject templatePrefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(originalPrefab);
        templatePrefabInstance.name = variantName;
        
        string newPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(originalPrefab))
                         + Path.DirectorySeparatorChar + templatePrefabInstance.name + ".prefab";
        newPath = AssetDatabase.GenerateUniqueAssetPath(newPath);
        prefabVariant = PrefabUtility.SaveAsPrefabAsset(templatePrefabInstance, newPath, out bool success);
        DestroyImmediate(templatePrefabInstance);
        
        return success;
    }
    
    protected abstract void InjectDataIntoPrefab(TPrefabData prefabData);
}
