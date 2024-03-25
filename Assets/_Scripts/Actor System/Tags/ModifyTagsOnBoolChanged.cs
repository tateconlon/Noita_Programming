using System.Collections.Generic;
using Sirenix.OdinInspector;
using ThirteenPixels.Soda;
using UnityEngine;

public class ModifyTagsOnBoolChanged : MonoBehaviour
{
    [SerializeField, Required] private ActorTagSet _actorTags;
    [SerializeField] private ScopedVariable<bool> _bool;
    [SerializeField] private List<Tag> _addTagsWhileTrue;
    [SerializeField] private List<Tag> _removeTagsWhileTrue;
    
    private void OnEnable()
    {
        _bool.onChangeValue.AddResponse(OnBoolChanged);
        
        if (_bool.value)
        {
            OnBoolTrue();
        }
    }
    
    private void OnBoolChanged(bool value)
    {
        if (value)
        {
            OnBoolTrue();
        }
        else
        {
            OnBoolFalse();
        }
    }
    
    private void OnBoolTrue()
    {
        foreach (Tag tagToRemove in _removeTagsWhileTrue)
        {
            _actorTags.Remove(tagToRemove);
        }
        
        foreach (Tag tagToAdd in _addTagsWhileTrue)
        {
            _actorTags.Add(tagToAdd);
        }
    }
    
    private void OnBoolFalse()
    {
        foreach (Tag tagToRemove in _addTagsWhileTrue)
        {
            _actorTags.Remove(tagToRemove);
        }
        
        foreach (Tag tagToAdd in _removeTagsWhileTrue)
        {
            _actorTags.Add(tagToAdd);
        }
    }
    
    private void OnDisable()
    {
        _bool.onChangeValue.RemoveResponse(OnBoolChanged);
        
        if (_bool.value)
        {
            OnBoolFalse();  // Undo the tag modifications of this component before disabling
        }
    }
    
    private void Reset()
    {
        _actorTags = GetComponentInParent<ActorTagSet>();
    }
}