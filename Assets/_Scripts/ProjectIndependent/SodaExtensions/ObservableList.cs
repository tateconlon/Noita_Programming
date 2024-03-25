using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ObservableList<T> : IList<T>
{
    [SerializeField]
    private List<T> _list = new();

    [SerializeField]
    public UnityEvent<T> ItemAdded = new();
    
    [SerializeField]
    public UnityEvent<T> ItemRemoved = new();

    public IEnumerator<T> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_list).GetEnumerator();
    }

    public void Add(T item)
    {
        _list.Add(item);
        ItemAdded.Invoke(item);
    }

    public void Clear()
    {
        List<T> copy = new(_list);
        
        _list.Clear();
        
        foreach (T item in copy)
        {
            ItemRemoved.Invoke(item);
        }
    }

    public bool Contains(T item)
    {
        return _list.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        bool result = _list.Remove(item);

        if (result)
        {
            ItemRemoved.Invoke(item);
        }

        return result;
    }

    public int Count => _list.Count;

    public bool IsReadOnly => false;

    public int IndexOf(T item)
    {
        return _list.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        _list.Insert(index, item);
        ItemAdded.Invoke(item);
    }

    public void RemoveAt(int index)
    {
        T item = this[index];
        
        _list.RemoveAt(index);
        ItemRemoved.Invoke(item);
    }

    public T this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }
}
