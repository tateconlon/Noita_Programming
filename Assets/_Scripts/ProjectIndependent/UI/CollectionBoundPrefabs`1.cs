using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class CollectionBoundPrefabs<TData, TBindable> : IReadOnlyDictionary<TData, TBindable> where TBindable : Component, IBindable<TData>
{
    [SerializeField] private Transform _prefabsHierarchyRoot;
    [Tooltip("Enable to destroy placeholder prefabs under hierarchy root")]
    [SerializeField] private bool _deleteRootChildrenOnInit = false;
    [SerializeField] private TBindable _prefab;

    private bool _hasRunInit = false;
    private readonly Dictionary<TData, TBindable> _dataToBindables = new();

    private void Init()
    {
        _hasRunInit = true;

        if (_deleteRootChildrenOnInit)
        {
            foreach (Transform child in _prefabsHierarchyRoot)
            {
                Object.Destroy(child.gameObject);
            }
        }
    }
    
    public void Bind(IEnumerable<TData> collection)
    {
        if (!_hasRunInit)
        {
            Init();
        }
        
        foreach ((TData _, TBindable bindable) in _dataToBindables)
        {
            Object.Destroy(bindable.gameObject);
        }
        
        _dataToBindables.Clear();

        foreach (TData newElement in collection)
        {
            TBindable newPrefab = Object.Instantiate(_prefab, _prefabsHierarchyRoot);
            newPrefab.Bind(newElement);
            
            _dataToBindables[newElement] = newPrefab;
        }
    }

    #region IReadOnlyDictionary
    
    public IEnumerator<KeyValuePair<TData, TBindable>> GetEnumerator()
    {
        return _dataToBindables.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_dataToBindables).GetEnumerator();
    }

    public int Count => _dataToBindables.Count;

    public bool ContainsKey(TData key)
    {
        return _dataToBindables.ContainsKey(key);
    }

    public bool TryGetValue(TData key, out TBindable value)
    {
        return _dataToBindables.TryGetValue(key, out value);
    }

    public TBindable this[TData key] => _dataToBindables[key];

    public IEnumerable<TData> Keys => _dataToBindables.Keys;

    public IEnumerable<TBindable> Values => _dataToBindables.Values;
    
    #endregion
}
