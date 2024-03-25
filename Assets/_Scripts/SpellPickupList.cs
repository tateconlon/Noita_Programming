using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell Item List", menuName = "ScriptableObject/Wand System/Spell Pickup List")]
public class SpellPickupList : ScriptableObject, IList<SpellPickupList.RoomSpellDefinition>
{
    [ListDrawerSettings(ShowPaging = false, ShowFoldout = true)]
    public List<RoomSpellDefinition> list;

    [Serializable]
    public class RoomSpellDefinition
    {
        [SerializeField] public List<SpellDefinition> spellDefs;
    }
    public IEnumerator<RoomSpellDefinition> GetEnumerator()
    {
        return list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)list).GetEnumerator();
    }

    public void Add(RoomSpellDefinition item)
    {
        list.Add(item);
    }

    public void Clear()
    {
        list.Clear();
    }

    public bool Contains(RoomSpellDefinition item)
    {
        return list.Contains(item);
    }

    public void CopyTo(RoomSpellDefinition[] array, int arrayIndex)
    {
        list.CopyTo(array, arrayIndex);
    }

    public bool Remove(RoomSpellDefinition item)
    {
        return list.Remove(item);
    }

    public int Count => list.Count;

    public bool IsReadOnly => ((ICollection<SpellDefinition>)list).IsReadOnly;

    public int IndexOf(RoomSpellDefinition item)
    {
        return list.IndexOf(item);
    }

    public void Insert(int index, RoomSpellDefinition item)
    {
        list.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        list.RemoveAt(index);
    }

    public RoomSpellDefinition this[int index]
    {
        get => list[index];
        set => list[index] = value;
    }
}
