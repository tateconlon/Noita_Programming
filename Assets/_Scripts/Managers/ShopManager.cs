using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject _shopRoot;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void EnterShop()
    {
        _shopRoot.SetActive(true);
    }
    
    public void ExitShop()
    {
        _shopRoot.SetActive(false);
    }
}
