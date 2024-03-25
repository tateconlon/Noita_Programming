using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class MoveRectTransformWhilePressed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField, Required] private RectTransform _rectTransform;
    [SerializeField] private Vector2 _pressedMoveDelta = new(0.0f, -5.0f);
    
    private bool _isClickedDown = false;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        _isClickedDown = true;

        _rectTransform.anchoredPosition += _pressedMoveDelta;
        _rectTransform.sizeDelta += _pressedMoveDelta;
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isClickedDown) return;
        _isClickedDown = false;
        
        _rectTransform.anchoredPosition -= _pressedMoveDelta;
        _rectTransform.sizeDelta -= _pressedMoveDelta;
    }
    
    private void Reset()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
}