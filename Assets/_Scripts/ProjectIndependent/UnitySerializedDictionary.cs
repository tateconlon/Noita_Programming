using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// From: https://odininspector.com/tutorials/serialize-anything/serializing-dictionaries

[Serializable]
public abstract class UnitySerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField, HideInInspector]
    [FormerlySerializedAs("keyData")]
    private List<TKey> _keyData = new();
	
    // NOTE: Unity can't serialize Lists of Lists, so if you want your dictionary values to be Lists/arrays,
    // you'll need to make a wrapper class and have UnitySerializedDictionary<TKey, WrapperClass>
    [SerializeField, HideInInspector]
    [FormerlySerializedAs("valueData")]
    private List<TValue> _valueData = new();

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        Clear();
        for (int i = 0; i < _keyData.Count && i < _valueData.Count; i++)
        {
            this[_keyData[i]] = _valueData[i];
        }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        _keyData.Clear();
        _valueData.Clear();

        foreach ((TKey key, TValue value) in this)
        {
            _keyData.Add(key);
            _valueData.Add(value);
        }
    }
}