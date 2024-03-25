using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "URL Constant", menuName = "ScriptableObject/URL Constant", order = 0)]
public class UrlConstant : ScriptableObject
{
    [SerializeField]
    public string displayName;
    
    [SerializeField]
    public Sprite icon;
    
    [SerializeField]
    public string url;
    
    [Button]
    public void OpenInBrowser()
    {
        Application.OpenURL(url);
    }
}
