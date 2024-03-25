using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;

[RequireComponent(typeof(AudioListener))]
public class AudioListenerPriorityEnabler : MonoBehaviour
{
    [SerializeField, Tooltip("The AudioListener with the highest priority will be enabled")]
    private int _priority = 0;

    [SerializeField]
    private AudioListener _listener;
    
    private static readonly ObservableCollection<AudioListenerPriorityEnabler> Instances = new();
    
    private void OnEnable()
    {
        Instances.CollectionChanged += OnInstancesChanged;
        Instances.Add(this);
    }
    
    private void OnInstancesChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        _listener.enabled = IsHighestPriority();
    }

    private bool IsHighestPriority()
    {
        foreach (AudioListenerPriorityEnabler instance in Instances)
        {
            if (instance == this) continue;
            
            if (instance._priority > _priority)
            {
                return false;
            }
            
            // Tiebreak on instance ID if two instances have the same priority - deterministic result
            if (instance._priority == _priority && instance.GetInstanceID() > GetInstanceID())
            {
                return false;
            }
        }

        return true;
    }

    private void OnDisable()
    {
        Instances.CollectionChanged -= OnInstancesChanged;
        Instances.Remove(this);
    }

    private void Reset()
    {
        _listener = GetComponent<AudioListener>();
    }
}
