using System.Collections;
using ThirteenPixels.Soda;
using UnityEngine;

public class ReactivateAfterDuration : MonoBehaviour
{
    [SerializeField] private ScopedVariable<float> _disabledDuration;

    private Coroutine _coroutine;
    
    private void OnDisable()
    {
        if (this.IsSceneUnloading()) return;  // Ignore if this is being destroyed
        
        _coroutine = CoroutineRunner.Run(ActivateAfterDuration());
    }

    private IEnumerator ActivateAfterDuration()
    {
        yield return new WaitForSeconds(_disabledDuration.value);
        
        gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        if (_coroutine != null)
        {
            CoroutineRunner.Stop(_coroutine);
        }
    }
}
