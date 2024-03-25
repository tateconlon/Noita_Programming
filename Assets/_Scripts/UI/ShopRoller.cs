using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopRoller : MonoBehaviour
{
    public List<RoomRewards> roomRewards = new();
    
    public static ShopRoller Instance;
    public static event Action<RoomRewards> OnRoll;
    public static event Action OnInit;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        GameManager.Instance.Shop.OnSetIsActive += OnActivateShop;
        OnActivateShop(GameManager.Instance.Shop.IsActive);
    }

    private void OnDestroy()
    {
        GameManager.Instance.Shop.OnSetIsActive -= OnActivateShop;
    }

    private void OnActivateShop(bool isActive)
    {
        gameObject.SetActive(isActive);
        if (isActive)
        {
            int rewardIndex = GameManager.Instance.CurWaveIndex;
            
            RoomRewards rewards = roomRewards[rewardIndex];
            OnRoll?.Invoke(rewards);
        }
    }
    
    //Intermediate class so we can have a list of lists in the inspector
    [Serializable]
    public class RoomRewards
    {
        [SerializeField] public List<SpellItemV2> rewards;
    }
}