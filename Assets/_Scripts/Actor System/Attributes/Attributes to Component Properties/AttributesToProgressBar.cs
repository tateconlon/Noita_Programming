using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

public class AttributesToProgressBar : MonoBehaviour
{
    [SerializeField, Required] private MMProgressBar _progressBar;
    [SerializeField, Required] private AttributeComponent _curAttribute;
    [SerializeField, Required] private AttributeComponent _maxAttribute;
    
    private void OnEnable()
    {
        _curAttribute.OnChangeCurValue += OnChangeCurValue;
        _maxAttribute.OnChangeCurValue += OnChangeCurValue;
        
        OnChangeCurValue(new AttributeComponent.ChangeValueParams(_curAttribute));
    }
    
    private void OnChangeCurValue(AttributeComponent.ChangeValueParams changeValueParams)
    {
        _progressBar.UpdateBar(_curAttribute.curValue, 0f, _maxAttribute.curValue);
        
        _progressBar.gameObject.SetActive(_curAttribute.curValue > 0f && _curAttribute.curValue < _maxAttribute.curValue);
    }
    
    private void OnDisable()
    {
        _curAttribute.OnChangeCurValue -= OnChangeCurValue;
        _maxAttribute.OnChangeCurValue -= OnChangeCurValue;
    }
    
    private void Reset()
    {
        _progressBar = GetComponent<MMProgressBar>();
    }
}
