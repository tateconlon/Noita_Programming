using Sirenix.OdinInspector;
using ThirteenPixels.Soda;
using UnityEngine;
using UnityEngine.UI;

public class GlobalFloatToImageFill : MonoBehaviour
{
    [SerializeField, Required] private Image _image;
    [SerializeField, Required] private GlobalFloat _globalFloat;
    
    private void OnEnable()
    {
        _globalFloat.onChange.AddResponseAndInvoke(OnGlobalFloatChange);
    }
    
    private void OnGlobalFloatChange(float value)
    {
        _image.fillAmount = value;
    }
    
    private void OnDisable()
    {
        _globalFloat.onChange.RemoveResponse(OnGlobalFloatChange);
    }
    
    private void Reset()
    {
        _image = GetComponent<Image>();
    }
}