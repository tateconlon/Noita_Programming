using System.Collections.Generic;
using UnityEngine;

public class Dragger2D : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Vector2 dragPoint;

    Vector2 DragPointWorld => rb.transform.TransformPoint(dragPoint);

    Draggable2D _curDraggable;
    
    void Update()
    {
        if (_curDraggable == null)
        {
            // TODO: replace with more advanced input handling
            if (Input.GetMouseButtonDown(0))
            {
                if (TryGetDraggableAtPoint(DragPointWorld, out _curDraggable))
                {
                    _curDraggable.BeginDrag(rb, DragPointWorld);
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                _curDraggable.EndDrag(rb);
                _curDraggable = null;
            }
        }
    }

    static bool TryGetDraggableAtPoint(Vector2 worldPos, out Draggable2D draggable)
    {
        ContactFilter2D contactFilter = new();

        contactFilter = contactFilter.NoFilter();  // TODO: change to use filtering

        List<Collider2D> results = new();  // OPTIMIZE: use a no alloc version

        Physics2D.OverlapPoint(worldPos, contactFilter, results);

        foreach (Collider2D col in results)
        {
            if (col.TryGetComponent(out draggable)) return true;
        }

        draggable = null;
        return false;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(rb.transform.position, DragPointWorld);
    }
}
