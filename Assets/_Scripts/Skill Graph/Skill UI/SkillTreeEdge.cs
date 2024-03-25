using UnityEngine;
using UnityEngine.UI.Extensions;

[RequireComponent(typeof(UILineRenderer))]
public class SkillTreeEdge : MonoBehaviour, IBindable<SkillTreeBuilder.Edge>
{
    [SerializeField] private UILineRenderer _lineRenderer;
    [SerializeField] private RectTransform _arrow;
    
    public SkillTreeBuilder.Edge BoundTarget { get; private set; }
    
    public void Bind(SkillTreeBuilder.Edge target)
    {
        BoundTarget = target;
        
        // Zero out transform position so points set below have no offset
        ((RectTransform)transform).anchoredPosition = Vector2.zero;
        
        _lineRenderer.Points = new Vector2[2];
        _lineRenderer.Points[0] = transform.InverseTransformPoint(target.source._rightEdgeConnection.position);
        _lineRenderer.Points[1] = transform.InverseTransformPoint(target.destination._leftEdgeConnection.position);

        if (target.source.BoundTarget.node is TriggerNode)
        {
            _arrow.gameObject.SetActive(true);

            _arrow.localPosition = _lineRenderer.Points[1];
            _lineRenderer.Points[1] -= new Vector2(_arrow.rect.width, 0);
        }
        else
        {
            _arrow.gameObject.SetActive(false);
        }
    }

    private void Reset()
    {
        _lineRenderer = GetComponent<UILineRenderer>();
    }
}
