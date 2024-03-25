using UnityEngine;

public static class MonoBehaviourExtensions
{
    /// <summary>
    /// True if this MonoBehavior's scene is unloading as the result of unloading the scene, loading a new scene,
    /// or quitting the application.
    ///
    /// Use for cases like instantiating new GameObjects to prevent spawning in response to OnDestroy.
    /// </summary>
    public static bool IsSceneUnloading(this MonoBehaviour monoBehaviour)
    {
        return monoBehaviour.gameObject.scene.GetLoadingState() == UnityEngineExtensions.SceneLoadingState.Unloading;
    }
    
    public static bool TryGetComponentInParents<T>(this Component obj, out T component) where T : Component
    {
        return obj.gameObject.TryGetComponentInParents(out component);
    }

    public static bool TryGetComponentInParents<T>(this GameObject obj, out T component) where T : Component
    {
        component = obj.GetComponentInParent<T>();

        return component != null;
    }
}
