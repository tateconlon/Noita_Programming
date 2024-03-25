using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class WandV2 : MonoBehaviour
{
    
    public List<SpellItemV2> spells = new();
    
    [SerializeField] private float reloadTime = 0.4f;  //TODO TATE: Replace with a stat
    private bool canFire = true;
    
    [FoldoutGroup("Bindings",expanded:false)]
    [SerializeField] public Transform spellContainer;
    [FoldoutGroup("Bindings",expanded:false)]
    [SerializeField] private Transform firePoint;
    [FoldoutGroup("Bindings",expanded:false)]
    public StatsModHolder statsMod;
    public event Action OnReadyToCast;
    public static event Action OnWandRebuild;
    
    void Start()
    {
        Rebuild();
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime);
        canFire = true;
        OnReadyToCast?.Invoke();
    }
    
    public bool TryCast(Vector3 dir)
    {
        //if (!canFire) return false;
        
        // print($"Cast {firePoint.position}, {dir}");

        for (int i = 0; i < spells.Count; i++)
        {
            if (spells[i] != null)
            {
                return spells[i].TryCast(new SpellItemV2.CastCommand()
                    {
                        Pos = firePoint.position,
                        Dir = dir
                    },
                    statsMod[StatType.ManaRegen].Modify(3) * Time.deltaTime);
            }
        }

        return false;
    }
    
    [Button("Rebuild")]
    public void Rebuild()
    {
        foreach (SpellItemV2 spell in spells)
        {
            if (spell != null)
            {
                spell.ClearPass();
            }
        }
        
        foreach (SpellItemV2 spell in spells)
        {
            if (spell != null)
            {
                spell.FirstPass();
            }
        }
        
        foreach (SpellItemV2 spell in spells)
        {
            if (spell != null)
            {
                spell.SecondPass();
            }
        }
        
        foreach (SpellItemV2 spell in spells)
        {
            if (spell != null)
            {
                spell.ThirdPass();
            }
        }
        
        foreach (SpellItemV2 spell in spells)
        {
            if (spell != null)
            {
                spell.FourthPass();
            }
        }
        
        OnWandRebuild?.Invoke();
    }
}