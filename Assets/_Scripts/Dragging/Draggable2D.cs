using Sirenix.OdinInspector;
using UnityEngine;

public class Draggable2D : MonoBehaviour
{
    [ValidateInput(nameof(ValidateDragJoint), "Joint must be disabled to avoid anchoring to fixed point")]
    [SerializeField] AnchoredJoint2D dragJoint;
    
    // TODO: calculate/apply movement penalty to Dragger?

    public void BeginDrag(Rigidbody2D otherRigidbody, Vector2 otherAnchorWorldPos)
    {
        dragJoint.enabled = true;

        dragJoint.connectedBody = otherRigidbody;
        dragJoint.connectedAnchor = otherRigidbody.transform.InverseTransformPoint(otherAnchorWorldPos);

        Vector2 closestPoint = dragJoint.attachedRigidbody.ClosestPoint(otherAnchorWorldPos);
        dragJoint.anchor = dragJoint.attachedRigidbody.transform.InverseTransformPoint(closestPoint);
    }

    public void EndDrag(Rigidbody2D otherRigidbody)
    {
        dragJoint.enabled = false;
    }

    bool ValidateDragJoint(AnchoredJoint2D joint2D)
    {
        // If the joint is enabled, then it will anchor itself to a fixed point in space
        // See https://docs.unity3d.com/ScriptReference/Joint2D-connectedBody.html
        
        return dragJoint != null && !dragJoint.enabled;
    }
}
