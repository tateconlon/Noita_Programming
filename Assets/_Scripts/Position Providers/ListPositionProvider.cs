using System.Collections.Generic;
using UnityEngine;

public class ListPositionProvider : PositionProvider
{
    [SerializeField]
    private List<Transform> _transforms;

    protected override PositionRotation GetPositionRotationInternal()
    {
        return new PositionRotation(_transforms.GetRandom());
    }
}
