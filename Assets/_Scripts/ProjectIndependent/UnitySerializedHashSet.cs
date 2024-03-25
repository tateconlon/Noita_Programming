using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class UnitySerializedHashSet<T> : ISet<T>, ISerializationCallbackReceiver
{
    // NOTE: Unity can't serialize Lists of Lists, so if you want your elements to be Lists/arrays,
    // you'll need to make a wrapper class and have UnitySerializedHashSet<WrapperClass>
    [SerializeField, HideInInspector]
    private List<T> _elementData = new();
    
    private HashSet<T> _hashSet = new();

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        _hashSet = new HashSet<T>(_elementData);
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        _elementData.Clear();
        _elementData.AddRange(_hashSet);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _hashSet.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_hashSet).GetEnumerator();
    }

    void ICollection<T>.Add(T item)
    {
        _hashSet.Add(item);
    }

    public void ExceptWith(IEnumerable<T> other)
    {
        _hashSet.ExceptWith(other);
    }

    public void IntersectWith(IEnumerable<T> other)
    {
        _hashSet.IntersectWith(other);
    }

    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        return _hashSet.IsProperSubsetOf(other);
    }

    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        return _hashSet.IsProperSupersetOf(other);
    }

    public bool IsSubsetOf(IEnumerable<T> other)
    {
        return _hashSet.IsSubsetOf(other);
    }

    public bool IsSupersetOf(IEnumerable<T> other)
    {
        return _hashSet.IsSupersetOf(other);
    }

    public bool Overlaps(IEnumerable<T> other)
    {
        return _hashSet.Overlaps(other);
    }

    public bool SetEquals(IEnumerable<T> other)
    {
        return _hashSet.SetEquals(other);
    }

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        _hashSet.SymmetricExceptWith(other);
    }

    public void UnionWith(IEnumerable<T> other)
    {
        _hashSet.UnionWith(other);
    }

    public bool Add(T item)
    {
        return _hashSet.Add(item);
    }

    public void Clear()
    {
        _hashSet.Clear();
    }

    public bool Contains(T item)
    {
        return _hashSet.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _hashSet.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        return _hashSet.Remove(item);
    }

    public int Count => _hashSet.Count;

    public bool IsReadOnly => ((ICollection<T>)_hashSet).IsReadOnly;
}