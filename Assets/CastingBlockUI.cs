using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class CastingBlockUI :  MonoBehaviour, IBindable<CastingBlock>
{
    [SerializeField] private CastingBlock _target;

    [SerializeField] private List<HUDSpellSlot> slots;
    [FoldoutGroup("Bindings", expanded: false)]
    [SerializeField] private HUDSpellSlot slotPrefab;
    [FoldoutGroup("Bindings", expanded: false)]
    [SerializeField] private Image triggerIcon;
    [FoldoutGroup("Bindings", expanded: false)]
    [SerializeField] private Image arrowIcon;
    [FoldoutGroup("Bindings", expanded: false)]
    [SerializeField] private GameObject invalidWarningGO;
    public CastingBlock BoundTarget
    {
        get { return _target; }
    }

    public void Bind(CastingBlock target)
    {
        _target = target;
        
        invalidWarningGO.SetActive(!_target.IsValid());

        foreach (HUDSpellSlot hudSpellSlot in slots)
        {
            GameObject.Destroy(hudSpellSlot.gameObject);
        }
        slots.Clear();

        if(_target == null) { return; }

        for (int i = 0; i < _target.getSpells().Length; i++)
        {
            HUDSpellSlot slot = GameObject.Instantiate(slotPrefab, this.transform);
            slot.Bind(_target.getSpells()[i], _target, i);
            slots.Add(slot);
        }

        if (_target.ParentSpell != null)
        {
            triggerIcon.gameObject.SetActive(true);
            arrowIcon.gameObject.SetActive(true);
            triggerIcon.sprite = _target.ParentSpell.definition.icon;
        }
        else
        {
            triggerIcon.gameObject.SetActive(false);
            arrowIcon.gameObject.SetActive(false);
        }
        
    }
}
