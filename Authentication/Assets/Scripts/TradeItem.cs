using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TradeItem : MonoBehaviour
{
    public GameObject tradeCanvas;
    public TextMeshProUGUI inventoryText;
    [HideInInspector]
    public List<ItemInstance> inventory;
    [HideInInspector]
    public List<CatalogItem> catalog;

    public static TradeItem instance;
    void Awake() { instance = this; }

    public UnityEvent onRefreshUI;

    public void OnLoggedIn()
    {
        tradeCanvas.SetActive(true);
        if (onRefreshUI != null)
            onRefreshUI.Invoke();
    }

    public void GetInventory()
    {

    }

    public void GetCatalog()
    {

    }
}
