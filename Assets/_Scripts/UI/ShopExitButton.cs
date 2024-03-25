using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopExitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static event Action ExitShopButtonClick;
    public void OnClick()
    {
        ExitShopButtonClick?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseOverVisuals(transform);
    }
    
    void MouseOverVisuals(Transform trans)
    {
        trans.DOScale(Vector3.one * 1.1f, 0.1f).SetEase(Ease.OutBack);
    }
    
    void MouseOverVisualsBack(Transform trans)
    {
        trans.DOScale(Vector3.one , 0.1f).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseOverVisualsBack(transform);
    }
}
