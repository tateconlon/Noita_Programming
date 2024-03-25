using System.Collections;
using System.Collections.Generic;
using NodeCanvas.BehaviourTrees;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    [ShowInInspector]
    [Header("Filled On Awake")]
    private List<Room> rooms = new();

    [FoldoutGroup("Bindings", expanded: true)]
    public SpellPickupList itemList;
    
    void Awake()
    {
        Room[] roomsArr = GameObject.FindObjectsByType<Room>(FindObjectsSortMode.None);
        rooms = new List<Room>(roomsArr);
        rooms.Sort();

        //Letting this possibly error out on purpose so we know what's wrong when it fails catastrophically
        for (int i = 0; i < rooms.Count; i++)
        {
            rooms[i].spellPickup1.Bind(new SpellItem(itemList[i].spellDefs[0]));
            rooms[i].spellPickup1.isRoomPickup = true;
            
            rooms[i].spellPickup2.Bind(new SpellItem(itemList[i].spellDefs[1]));
            rooms[i].spellPickup2.isRoomPickup = true;
        }
    }
}
