using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class PointerEnterHandler : MonoBehaviour, IPointerEnterHandler
{
    [FormerlySerializedAs("onPointerDown")] [InfoBox("Use this instead of Unity's EventTrigger to avoid eating all other IEventSystemHandler events")]
    public UnityEvent<PointerEventData> onPointerEnter;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter.Invoke(eventData);
    }
}
