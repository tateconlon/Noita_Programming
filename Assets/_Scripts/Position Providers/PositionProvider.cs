using System;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class PositionProvider : MonoBehaviour
{
    [SerializeField] private bool _logExceptions = false;
    
    public bool TryGetPositionRotation(out PositionRotation positionRotation)
    {
        try
        {
            positionRotation = GetPositionRotationInternal();
            return true;
        }
        catch (Exception exception)
        {
            if (_logExceptions)
            {
                Debug.LogException(exception, this);
            }

            positionRotation = default;
            return false;
        }
    }

    protected abstract PositionRotation GetPositionRotationInternal();

    public bool TryGetPosition(out Vector3 position)
    {
        if (TryGetPositionRotation(out PositionRotation positionRotation))
        {
            position = positionRotation.Position;
            return true;
        }
        
        position = Vector3.zero;
        return false;
    }
    
    [Button]
    private void TestDrawPositions(int numRaycasts = 10)
    {
        for (int i = 0; i < numRaycasts; i++)
        {
            if (TryGetPositionRotation(out PositionRotation posRot))
            {
                Debug.DrawRay(posRot.Position, posRot.Rotation * Vector3.forward, Color.red, 3.0f);
            }
        }
    }
}

[Serializable]
public struct PositionRotation
{
    public PositionRotation(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }
    
    public PositionRotation(Transform transform)
    {
        Position = transform.position;
        Rotation = transform.rotation;
    }
    
    public Vector3 Position;
    public Quaternion Rotation;
}
