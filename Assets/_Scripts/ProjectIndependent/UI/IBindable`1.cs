using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBindable<T>
{
    /// <summary>
    /// Most likely includes a "private set"
    /// </summary>
    public T BoundTarget { get; }
    
    public void Bind(T target);
}
