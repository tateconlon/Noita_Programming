using Sirenix.OdinInspector;
using ThirteenPixels.Soda;
using UnityEngine;

public class TransformToVariables2D : MonoBehaviour
{
    [SerializeField] bool setPosition;
    [ShowIf(nameof(setPosition))]
    [SerializeField] GlobalVector2 positionVariable;
    
    [SerializeField] bool setDirection;
    [ShowIf(nameof(setDirection))]
    [SerializeField] GlobalVector2 directionVariable;
    
    [SerializeField] bool setScale;
    [ShowIf(nameof(setScale))]
    [SerializeField] GlobalVector2 scaleVariable;

    void Update()
    {
        if (setPosition)
        {
            positionVariable.value = transform.position;
        }

        if (setDirection)
        {
            directionVariable.value = transform.right;
        }

        if (setScale)
        {
            scaleVariable.value = transform.localScale;
        }
    }
}
