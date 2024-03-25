using System.Collections;
using UnityEngine;

public static class CounterBoolExtensions
{
    public static void IncrementForDuration(this CounterBool counterBool, float duration, MonoBehaviour host)
    {
        host.StartCoroutine(IncrementCounterBoolCR(counterBool, duration));
    }
    
    private static IEnumerator IncrementCounterBoolCR(CounterBool counterBool, float duration)
    {
        counterBool.Increment();
        
        yield return new WaitForSeconds(duration);
        
        counterBool.Decrement();
    }
}