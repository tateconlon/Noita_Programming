using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CastingBlockDepthUI : MonoBehaviour, IBindable<List<CastingBlock>>
{
    [SerializeField] private List<CastingBlock> _target;
    public List<CastingBlock> BoundTarget
    {
        get { return _target; }
    }

    [SerializeField] private List<CastingBlockUI> castingBlocks;
    
    [FoldoutGroup("Bindings",expanded:false)]
    [SerializeField] private CastingBlockUI castingBlockUIPrefab;
    
    //This could be a list of CastingBlocks or a list of SpellItems where you get the CastingBlock list from the spell items
    public void Bind(List<CastingBlock> target)
    {
        _target = target;
        
        foreach (CastingBlockUI castingBlockUI in castingBlocks)
        {
            GameObject.Destroy(castingBlockUI.gameObject);
        }
        castingBlocks.Clear();
        
        if(_target == null) {return;}

        for (int i = 0; i < _target.Count; i++)
        {
            CastingBlockUI castingBlockUI = GameObject.Instantiate(castingBlockUIPrefab, this.transform);
            castingBlockUI.Bind(_target[i]);
            castingBlocks.Add(castingBlockUI);
        }

    }
}
