using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class TimerToImageFill : MonoBehaviour
{
    [SerializeField, Required] private Image _image;
    [SerializeField, Required] private Timer _timer;
    [SerializeField] private FillBehavior _fillBehavior = FillBehavior.TimeElapsedToFillAmount;
    
    private void Update()
    {
        switch (_fillBehavior)
        {
            case FillBehavior.TimeElapsedToFillAmount:
                _image.fillAmount = _timer == null ? 0f : _timer.TimeElapsedNormalized;
                break;
            case FillBehavior.TimeRemainingToFillAmount:
                _image.fillAmount = _timer == null ? 1f : _timer.TimeRemainingNormalized;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private enum FillBehavior
    {
        TimeElapsedToFillAmount,
        TimeRemainingToFillAmount
    }
}