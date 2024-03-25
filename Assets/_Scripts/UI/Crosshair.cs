using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private GameObject _visibleElements;
    
    private void Update()
    {
        RefreshVisible();
        
        if (Camera.main == null) return;
        
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(PointerManager.PointerPosition);
        targetPos.z = transform.position.z;  // Don't modify Z position - will be set to Z pos of camera plane
        
        transform.position = targetPos;
    }
    
    private void RefreshVisible()
    {
        bool showVisibleElements = !PauseManager.IsPaused.Value &&
                                   !PointerManager.IsOverUi &&
                                   !MouseInventorySlotV2.main.isHolding;
        
        _visibleElements.SetActive(showVisibleElements);
        Cursor.visible = !showVisibleElements;
    }
    
    private void OnDestroy()
    {
        Cursor.visible = true;
    }
}
