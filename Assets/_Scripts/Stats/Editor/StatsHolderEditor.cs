using System.Drawing.Printing;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StatsModHolder))]
public class StatsHolderEditor : Editor
{
    private StatChange tempStatChange = new StatChange();

    public override void OnInspectorGUI() {
        StatsModHolder statsModHolder = (StatsModHolder)target;

        bool noStatModsFlag = true;
        for (int i = 0; i < (int)StatType.Count; i++)
        {
            StatType statType = (StatType)i;
            if(statsModHolder[statType] == null) {
                continue;
            }

            noStatModsFlag = false;
            
            if(statsModHolder[statType].FlatBonus.IsApprox(0) && statsModHolder[statType].MultiplierBonus == 1 && statsModHolder[statType].MultiplierReduction == 1) {
                continue;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(statType.ToString(), GUILayout.Width(100)); // Adjust the width as needed
            EditorGUILayout.LabelField($"| A: {statsModHolder[statType].FlatBonus}", GUILayout.Width(55)); // Adjust the width as needed
            EditorGUILayout.LabelField($"| M: {statsModHolder[statType].MultiplierBonus:F2}", GUILayout.Width(55)); // Adjust the width as needed
            EditorGUILayout.LabelField($"| M Red: {statsModHolder[statType].MultiplierReduction:F2}", GUILayout.Width(200)); // Adjust the width as needed
            EditorGUILayout.EndHorizontal();
        }
        
        if (!EditorApplication.isPlaying && noStatModsFlag)
        {
            EditorGUILayout.LabelField("No Mods When Game Not Running");
        }
        else if(noStatModsFlag)
        {
            EditorGUILayout.LabelField("Stat Mods: 0");
        }

        if (EditorApplication.isPlaying)
        {
            // Draw a divider line
            GUILayout.Space(10);
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            rect.height = 1;
            EditorGUI.DrawRect(rect, Color.gray);
            GUILayout.Space(10);
        
            tempStatChange.type = (StatType)EditorGUILayout.EnumPopup("Stat Type", tempStatChange.type);
            tempStatChange.isFlatMod = EditorGUILayout.Toggle("Is Flat Mod", tempStatChange.isFlatMod);
        
            if (tempStatChange.isFlatMod) {
                tempStatChange.flatChange = EditorGUILayout.FloatField("Flat Value", tempStatChange.flatChange);
                tempStatChange.multChange = 0;   //Prevent accidental value carryover from when isFlatMod wasn't checked
            } else {
                tempStatChange.multChange = EditorGUILayout.FloatField("Change", tempStatChange.multChange);
                tempStatChange.flatChange = 0;   //Prevent accidental value carryover from when isFlatMod wasn't checked
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add StatChange"))
            {
                statsModHolder.ApplyStatChanges(new StatChange[] {tempStatChange});
                tempStatChange = new StatChange();
                
                //Remove focus from text field which fucks everything up since the value is still there
                //But it's not in the tempStatChange
                RemoveFocus();
            }
            if (GUILayout.Button("Remove StatChange"))
            {
                statsModHolder.RemoveStatChanges(new StatChange[] {tempStatChange});
                tempStatChange = new StatChange();
                
                //Remove focus from text field which fucks everything up since the value is still there
                //But it's not in the tempStatChange
                RemoveFocus();
            }
            GUILayout.EndHorizontal();
        }
        
        // Apply changes made in the editor
        if (GUI.changed) {
            EditorUtility.SetDirty(target);
        }
    }

    // void OnInspectorUpdate(StatsModHolder holder)
    // {
    //     if (holder == target)
    //     {
    //         Repaint();
    //     }
    // }

    void RemoveFocus()
    {
        GUI.SetNextControlName("DummyControl");
        EditorGUILayout.TextField("", GUIStyle.none);
        GUI.FocusControl("DummyControl");
    }
}