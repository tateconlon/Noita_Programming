using System;
using DG.Tweening;
using MoreMountains.Feedbacks;
using ThirteenPixels.Soda;
using UnityEngine;

public class UpdatePlayerMovement : MonoBehaviour
{
    [SerializeField] GlobalVector2 inputPosition;
    [SerializeField] GlobalVector2 tempHeartTargetPos;  // TODO: move this out
    [SerializeField] GameObject tempTarget;  // TODO: move this out
    [SerializeField] MMFeedbacks temptargetFeedbacks;
    [SerializeField] Vector2 offset;

    public ScopedVariable<float> radiusTargetDissapear;

    void OnEnable()
    {
        inputPosition.onChange.AddResponseAndInvoke(UpdatePosition);
    }

    void UpdatePosition(Vector2 newPosition)
    {
        transform.position = newPosition + offset;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            tempHeartTargetPos.value = ((Vector2)transform.position) - offset;
            
            if (tempTarget != null)
            {
                tempTarget.transform.position = tempHeartTargetPos.value;
                tempTarget.transform.localScale = Vector3.zero;
            
                temptargetFeedbacks?.PlayFeedbacks();
            
                DOTween.Sequence()
                    .Append(DOTween.To(x => tempTarget.transform.localScale = Vector3.one * x, 0, 1, 0.1f)
                        .SetEase(Ease.OutBack));
            }
        }
    }

    void OnDisable()
    {
        inputPosition.onChange.RemoveResponse(UpdatePosition);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position - (Vector3)offset);
    }
}
