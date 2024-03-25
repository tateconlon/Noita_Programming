using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(MMProgressBar))]
public class SpellItemToManaBar : MonoBehaviour, IBindable<SpellItemV2>
{
    private SpellItemV2 _target;
    public SpellItemV2 BoundTarget { get; }
    
    [FoldoutGroup("Bindings",expanded:false)]
    [SerializeField] private MMProgressBar _progressBar;

    void OnEnable()
    {
        SpellItemV2.OnShoot += OnShoot;
    }

    private void OnDisable()
    {
        SpellItemV2.OnShoot -= OnShoot;
    }

    public void Bind(SpellItemV2 target)
    {
        _target = target;
        _progressBar.GetComponent<Canvas>().enabled = _target != null;
    }

    public void Update()
    {
        //We don't do a prev Mana comparison check here because we want to update the bar even if the mana is the same
        //ex: If we instantly charged enough to shoot, mana will go from 0 to 0 but the OnShoot will set the bar to 1
        if (_target != null)
        {
            _progressBar.UpdateBar(_target.UI_manaCurr, 0, _target.UI_manaCost);
        }
    }

    //On Shoot set the bar to 1 before the mana is updated to 0
    //So we get the effect of the bar going from full to empty
    void OnShoot(SpellItemV2 spell)
    {
        if (spell == _target)
        {
            _progressBar.UpdateBar(1f,0f, 1f);
        }
    }
}