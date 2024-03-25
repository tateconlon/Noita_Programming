using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

public class SkillDefinitionSpreadsheet : OdinEditorWindow
{
    [MenuItem("Tools/View All Skill Definitions")]
    private static void OpenWindow()
    {
        GetWindow<SkillDefinitionSpreadsheet>().Show();
    }

    [TableList(ShowIndexLabels = false, AlwaysExpanded = true, DrawScrollView = true)]
    public List<SkillDefinition> SkillDefinitions = new();

    private void Awake()
    {
        SkillDefinitions.Clear();
        
        foreach (string guid in AssetDatabase.FindAssets("t:" + nameof(SkillDefinition)))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            SkillDefinitions.Add(AssetDatabase.LoadAssetAtPath<SkillDefinition>(path));
        }
    }
}