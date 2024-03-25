using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryLoader : MonoBehaviour
{
    public SpellItemList playerInventory;

    public int numOfNull = 6;
    // Start is called before the first frame update
    void Start()
    {
        playerInventory.maxSize = numOfNull;
    }
    
    
}
