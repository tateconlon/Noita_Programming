using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public static class UnityEngineExtensions
{
    public static SceneLoadingState GetLoadingState(this Scene scene)
    {
        switch (scene.loadingState)
        {
            case Scene.LoadingState.NotLoaded:
                return SceneLoadingState.NotLoaded;
            case Scene.LoadingState.Loading:
                return SceneLoadingState.Loading;
            case Scene.LoadingState.Loaded:
                return SceneLoadingState.Loaded;
            case Scene.LoadingState.Unloading:
                return SceneLoadingState.Unloading;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public enum SceneLoadingState
    {
        NotLoaded,
        Loading,
        Loaded,
        Unloading,
    }

    public static bool IncludesLayer(this LayerMask layerMask, int layer)
    {
        return (layerMask & (1 << layer)) != 0;
    }
    
    // From: https://docs.unity3d.com/ScriptReference/GameObject.FindGameObjectsWithTag.html
    public static bool TryFindClosestWithTag(Vector3 queryPos, string tag, GameObject ignoredGo, out GameObject closest)
    {
        GameObject[] taggedGameObjects = GameObject.FindGameObjectsWithTag(tag);
        
        closest = null;
        float distance = Mathf.Infinity;
        bool foundMatch = false;
        
        foreach (GameObject taggedGo in taggedGameObjects)
        {
            if (taggedGo == ignoredGo) continue;
            
            float curDistance = (taggedGo.transform.position - queryPos).sqrMagnitude;
            
            if (curDistance < distance)
            {
                closest = taggedGo;
                distance = curDistance;
                foundMatch = true;
            }
        }
        
        return foundMatch;
    }
    
    // From: https://docs.unity3d.com/ScriptReference/GameObject.FindGameObjectsWithTag.html
    public static bool TryFindClosestWithTag(Vector3 queryPos, string tag, ICollection<GameObject> ignored, out GameObject closest)
    {
        GameObject[] taggedGameObjects = GameObject.FindGameObjectsWithTag(tag);
        
        closest = null;
        float distance = Mathf.Infinity;
        bool foundMatch = false;
        
        foreach (GameObject taggedGo in taggedGameObjects)
        {
            if (ignored.Contains(taggedGo)) continue;
            
            float curDistance = (taggedGo.transform.position - queryPos).sqrMagnitude;
            
            if (curDistance < distance)
            {
                closest = taggedGo;
                distance = curDistance;
                foundMatch = true;
            }
        }
        
        return foundMatch;
    }
}
