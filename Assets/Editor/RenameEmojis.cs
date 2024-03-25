using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "RenameEmojis", menuName = "ScriptableObject/Editor/RenameEmojis", order = 0)]
public class RenameEmojis : ScriptableObject
{
    // Run this in your browser console at this site to get unicode --> names:
    // https://www.unicode.org/emoji/charts/full-emoji-list.html
    // const emoji = {};
    // for (const code of document.querySelectorAll('td.code')) {
    //   let key = code.firstChild.name;
    //   emoji[key] = code.parentElement.getElementsByClassName("name")[0].textContent;
    // }
    // window.emoji = emoji;
    [TextArea]
    [SerializeField] string emojiUnicodeToNameJson;
    
    [SerializeField] DefaultAsset folderOfEmojis;

    [InfoBox("This takes forever", InfoMessageType.Warning)]
    [Button]
    void Run()
    {
        Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(emojiUnicodeToNameJson);

        if (dictionary == null)
        {
            Debug.LogError("Could not parse JSON");
            return;
        }

        Dictionary<string, string> dictionaryCleaned = new();

        foreach ((string unicode, string friendlyName) in dictionary)
        {
            string unicodeCleaned = unicode.Replace('_', '-');
            
            string friendlyNameCleaned = friendlyName;

            foreach (char invalidFileNameChar in Path.GetInvalidFileNameChars())
            {
                friendlyNameCleaned = friendlyNameCleaned.Replace(invalidFileNameChar.ToString(), string.Empty);
            }

            dictionaryCleaned[unicodeCleaned] = friendlyNameCleaned;
        }
        
        List<string> paths = CodeHelpers.GetAssetPathsFromFolder(folderOfEmojis, "");
        
        AssetDatabase.DisallowAutoRefresh();

        foreach (string path in paths)
        {
            if (dictionaryCleaned.TryGetValue(Path.GetFileNameWithoutExtension(path), out string newName))
            {
                AssetDatabase.RenameAsset(path, newName);
            }
        }
        
        Debug.Log(paths.Count);
        
        AssetDatabase.SaveAssets();
        
        AssetDatabase.AllowAutoRefresh();
    }
}
