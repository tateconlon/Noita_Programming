using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ModifyTagsWhileDodging : MonoBehaviour
{
    [SerializeField, Required] private Actor _actor;
    [SerializeField, Required] private DodgeRollOnInput _dodgeRoller;
    [SerializeField] private List<Tag> _addTagsWhileDodging;
    [SerializeField] private List<Tag> _removeTagsWhileDodging;
    
    private void OnEnable()
    {
        _dodgeRoller.DodgeStarted += OnDodgeStarted;
        _dodgeRoller.DodgeEnded += OnDodgeEnded;
        
        if (_dodgeRoller.IsDodging)
        {
            OnDodgeStarted();  // If this is being enabled while already dodging, retroactively modify tags
        }
    }
    
    private void OnDodgeStarted()
    {
        foreach (Tag tagToRemove in _removeTagsWhileDodging)
        {
            //_actor.Tags.Remove(tagToRemove);
        }
        
        foreach (Tag tagToAdd in _addTagsWhileDodging)
        {
            //_actor.Tags.Add(tagToAdd);
        }
    }
    
    private void OnDodgeEnded()
    {
        foreach (Tag tagToRemove in _addTagsWhileDodging)
        {
            //_actor.Tags.Remove(tagToRemove);
        }
        
        foreach (Tag tagToAdd in _removeTagsWhileDodging)
        {
            //_actor.Tags.Add(tagToAdd);
        }
    }
    
    private void OnDisable()
    {
        _dodgeRoller.DodgeStarted -= OnDodgeStarted;
        _dodgeRoller.DodgeEnded -= OnDodgeEnded;
        
        if (_dodgeRoller.IsDodging)
        {
            OnDodgeEnded();  // If this is being disabled mid-dodge, undo tag modifications before disabling
        }
    }
    
    private void Reset()
    {
        _actor = GetComponentInParent<Actor>();
        _dodgeRoller = GetComponentInParent<DodgeRollOnInput>();
    }
}