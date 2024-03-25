using System;
using Sirenix.OdinInspector;

public class GameEvent<TParams>
{
    public event Action<TParams> OnRaise;
    
    [Button(ButtonStyle.CompactBox, Expanded = true)]
    public void Raise(TParams gameEventParams)
    {
        OnRaise?.Invoke(gameEventParams);
    }
}
