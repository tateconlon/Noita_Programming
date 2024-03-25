using Sirenix.OdinInspector;
using UnityEngine;

public class TransformSpring : Spring
{
    [SerializeField] Transform targetTransform;

    [SerializeField] bool modifyPos = true;
    [ShowIf(nameof(modifyPos)), SerializeField] Vector3 posMult = Vector3.one;
    [ShowIf(nameof(modifyPos)), SerializeField] Vector3 posAdd = Vector3.zero;
    
    [SerializeField] bool modifyRot = true;
    [ShowIf(nameof(modifyRot)), SerializeField] Vector3 rotMult = Vector3.one;
    [ShowIf(nameof(modifyRot)), SerializeField] Vector3 rotAdd = Vector3.zero;
    
    [SerializeField] bool modifyScale = true;
    [ShowIf(nameof(modifyScale)), SerializeField] Vector3 scaleMult = Vector3.one;
    [ShowIf(nameof(modifyScale)), SerializeField] Vector3 scaleAdd = Vector3.zero;

    bool _isRectTransform = false;
    RectTransform _targetRectTransform;

    protected override void Awake()
    {
        base.Awake();

        if (targetTransform is RectTransform rectTransform)
        {
            _isRectTransform = true;
            _targetRectTransform = rectTransform;
        }
    }
    
    protected override void OnValueChanged(float prev, float cur)
    {
        if (modifyPos)
        {
            Vector3 newPos = Value * posMult + posAdd;
            
            if (_isRectTransform)
            {
                _targetRectTransform.anchoredPosition = newPos;
            }
            else
            {
                targetTransform.localPosition = newPos;
            }
        }
        if (modifyRot)
        {
            targetTransform.localRotation = Quaternion.Euler(Value * rotMult + rotAdd);
        }
        if (modifyScale)
        {
            targetTransform.localScale = Value * scaleMult + scaleAdd;
        }
    }
}
