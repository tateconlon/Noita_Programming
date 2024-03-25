using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using Object = UnityEngine.Object;

// TODO: REQUIRES THIS PACKAGE: https://github.com/Unity-Technologies/com.unity.search.extensions
// and this script needs to be placed inside the com.unity.search.extensions.editor asmdef using an asmref,
// see how here: https://docs.unity3d.com/Manual/class-AssemblyDefinitionReferenceImporter.html

// NOTE: Future work would ideally be able to index and draw headers for individual components, but that would be an
// entirely different approach both for searching and for drawing headers.

/// <summary>
/// Expands UnityEngine.Object headers to show a dropdown list of usages/references to the target Object.
/// Inspired by the "Code Vision usages" functionality in Rider/JetBrains IDEs.
/// Makes use of the new Unity Search indexing and API: https://docs.unity3d.com/Manual/search-overview.html
/// </summary>
[InitializeOnLoadAttribute]
static class ObjectUsagesHeader
{
    // TODO: Limit the size of this cache. Right now it gets cleared on domain reload which is often enough for me.
    private static readonly Dictionary<Editor, Object[]> UsagesCache = new();
    
    private static bool _expandDropdown = false;
    
    // Copied from UnityEditor.Search.Dependency.FindUsages()
    private static readonly string[] SearchProviderIds = { "dep", "scene", "asset", "adb" };
    
    static ObjectUsagesHeader()
    {
        //Breaks the play session if it fails (which it seems to a lot)
        // Editor.finishedDefaultHeaderGUI -= AppendUsagesHeader;
        // Editor.finishedDefaultHeaderGUI += AppendUsagesHeader;
    }
    
    private static void AppendUsagesHeader(Editor editor)
    {
        if (!EditorUtility.IsPersistent(editor.target)) return;
        
        EditorGUILayout.BeginVertical(new GUIStyle() { padding = new RectOffset(20, 5, 0, 0) });
        
        if (UsagesCache.TryGetValue(editor, out Object[] usages))
        {
            if (usages != null)  // Cache hit, search has completed and results are ready to query
            {
                DrawUsages(usages);
            }
            else  // Cache hit, but search is still in progress
            {
                EditorGUILayout.LabelField("Finding usages...");
            }
        }
        else  // Cache miss, start search
        {
            UsagesCache[editor] = null;
            
            StartSearch(editor);
            
            EditorGUILayout.LabelField("Finding usages...");
            
            _expandDropdown = false;  // Hacky af but gets the intended behavior ;)
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private static void DrawUsages(Object[] usages)
    {
        string foldoutLabel = usages.Length == 1 ? "1 usage" : $"{usages.Length} usages";
        
        _expandDropdown = EditorGUILayout.BeginFoldoutHeaderGroup(_expandDropdown, foldoutLabel, null, ShowContextMenu);
        
        if (_expandDropdown)
        {
            using (new EditorGUI.DisabledScope(true))  // Disable assigning anything to these ObjectFields
            {
                foreach (Object usage in usages)
                {
                    if(usage == null) continue;
                    EditorGUILayout.ObjectField(usage, usage.GetType(), true);
                }
            }
        }
        
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
    
    private static void ShowContextMenu(Rect position)
    {
        GenericMenu menu = new();
        
        if (!Dependency.HasIndex())
        {
            menu.AddItem(new GUIContent("Build Dependency Index"), false, Dependency.Build);
        }
        else if (Dependency.HasUpdate() && Dependency.IsReady())
        {
            menu.AddItem(new GUIContent("Update Dependency Index"), false, () => Dependency.Update(null));
        }
        else
        {
            menu.AddDisabledItem(new GUIContent("Update Dependency Index"));
        }
        
        menu.DropDown(position);
    }
    
    private static void StartSearch(Editor editor)
    {
        try
        {
            string path = UnityEditor.Search.SearchUtils.GetObjectPath(editor.target);
        
            SearchContext searchContext = SearchService.CreateContext(SearchProviderIds, $"ref=\"{path}\"");
        
            SearchService.Request(searchContext, (_, list) => OnSearchCompleted(editor, list));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    private static void OnSearchCompleted(Editor editor, IList<SearchItem> searchItems)
    {
        Object[] usages = new Object[searchItems.Count];
        
        for (int i = 0; i < searchItems.Count; i++)
        {
            usages[i] = searchItems[i].ToObject();
        }
        
        UsagesCache[editor] = usages;
    }
}