using System.Collections;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;

    private void Awake()
    {
        Debug.Assert(_instance == null);

        _instance = this;
    }

    /// <summary>
    /// Use this to run Coroutines on MonoBehaviours on disabled GameObjects
    /// </summary>
    /// <param name="routine"></param>
    /// <returns></returns>
    public static Coroutine Run(IEnumerator routine)
    {
        if (_instance == null)
        {
            Debug.Assert(_instance != null);
            return null;
        }
        
        return _instance.StartCoroutine(routine);
    }

    public static void Stop(Coroutine coroutine)
    {
        if (_instance == null)
        {
            // This is fine since all coroutines will be stopped in OnDestroy anyways
            return;
        }
        
        if (coroutine != null)
        {
            _instance.StopCoroutine(coroutine);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
