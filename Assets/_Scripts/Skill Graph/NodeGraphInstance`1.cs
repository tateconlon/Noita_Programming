using UnityEngine;
using XNode;

[DefaultExecutionOrder(int.MinValue)]
public abstract class NodeGraphInstance : ScriptableObject
{
    public abstract NodeGraph BaseValue { get; }
    
    public abstract void ResetValue();
}

public abstract class NodeGraphInstance<T> : NodeGraphInstance where T : NodeGraph
{
    [SerializeField] private T _nodeGraphAsset;
    
    public T Value { get; private set; }
    
    public override NodeGraph BaseValue => Value;

    private void OnEnable()
    {
        if (Value != null) return;
        
        ResetValue();
    }
    
    public override void ResetValue()
    {
        if (_nodeGraphAsset == null)
        {
            Value = CreateInstance<T>();
        }
        else
        {
            Value = _nodeGraphAsset.Copy() as T;
        }
    }

    private void OnDisable()
    {
        if (Value != null)
        {
            if (Application.isPlaying)
            {
                Destroy(Value);
            }
            else
            {
                DestroyImmediate(Value);
            }
        }
    }
    
    // TODO: this could also handle saving/loading graphs from files
}
