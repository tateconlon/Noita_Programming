using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell Item List", menuName = "ScriptableObject/Wand System/Spell Item List", order = 1)]
public class SpellItemList : ScriptableObject, IList<SpellItem>
{
    [NonSerialized, ShowInInspector]   //Non Serialized since we want it cleared on start
    private List<SpellItem> innerList = new();

    //This is hard set to 0 since other things initialize this.
    //This is not future proof, eg: if we want to use SpellItemLists to hold data
    //SpellItemList right now just holds PlayState data, similar to a runtimeset.
    [NonSerialized, ShowInInspector]
    private int _maxSize = 0;  //We keep _maxSize to emulate a fixed # of slots in a wand
    
    public int maxSize
    {
        get { return _maxSize; }
        set
        {
            _maxSize = value;
            if (_maxSize > innerList.Count)
            {
                for (int i = _maxSize; i < innerList.Count; i++)
                {
                    innerList.RemoveAt(i);
                }
            }

            while (innerList.Count < _maxSize)
            {
                innerList.Add(null);
            }

            OnListChanged(); //Technically the wand got "shorter" or "longer", so we need to update
        }
    }

    public event Action OnModified;

    protected virtual void OnListChanged()
    {
        OnModified?.Invoke();
    }

    public SpellItem this[int index]
    {
        get { return innerList[index]; }
        set
        {
            innerList[index] = value;
            OnListChanged();
        }
    }

    public void Add(SpellItem item)
    {
        if (innerList.Count >= maxSize)
        {
            Debug.LogError($"Can't add to list. {innerList.Count} >= {maxSize}", this);
            return;
        }
        innerList.Add(item);
        OnListChanged();
    }

    public void Insert(int index, SpellItem item)
    {
        if (innerList.Count >= maxSize)
        {
            Debug.LogError($"Can't add to list. {innerList.Count} >= {maxSize}", this);
            return;
        }
        innerList.Insert(index, item);
        OnListChanged();
    }

    public bool Remove(SpellItem item)
    {
        bool result = innerList.Remove(item);
        if (result)
        {
            OnListChanged();
        }

        return result;
    }

    public int Count => innerList.Count;

    public bool IsReadOnly => ((ICollection<SpellItem>)innerList).IsReadOnly;

    public int IndexOf(SpellItem item) => innerList.IndexOf(item);

    public void Clear()
    {
        innerList.Clear();
        FillWithNull();
        OnListChanged();
    }
    void FillWithNull()
    {
        while (innerList.Count < _maxSize)
        {
            innerList.Add(null);
        }
        OnListChanged();
    }

    public bool Contains(SpellItem item) => innerList.Contains(item);

    public void CopyTo(SpellItem[] array, int arrayIndex) => innerList.CopyTo(array, arrayIndex);

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= innerList.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        innerList.RemoveAt(index);
        OnListChanged();
    }

    public IEnumerator<SpellItem> GetEnumerator() => innerList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)innerList).GetEnumerator();
}
