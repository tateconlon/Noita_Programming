using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class HUDWandWiggle : MonoBehaviour
{
    [SerializeField] private int slotIndex = -1;
    [SerializeField] private MMWiggle wiggle;
    // Start is called before the first frame update
    void Start()
    {
        SpellItemV2.OnShoot += Wiggle_OnShoot;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnDestroy()
    {
        SpellItemV2.OnShoot -= Wiggle_OnShoot;
    }
    
    private void Wiggle_OnShoot(SpellItemV2 spellItemV2)
    {
        if (slotIndex >= 0 && slotIndex < PlayerControllerV2.instance.wand.spells.Count)
        {
            if (PlayerControllerV2.instance.wand.spells[slotIndex] == spellItemV2)
            {
                wiggle.WigglePosition(0.2f);
                wiggle.WiggleRotation(0.2f);
                wiggle.WiggleScale(0.2f);
            }
        }
    }
}
