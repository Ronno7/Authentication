using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Trade : MonoBehaviour
{
    [Header("UI References")]
    public GameObject tradeCanvas;
    public TextMeshProUGUI inventoryText;
    public TextMeshProUGUI displayText;

    [HideInInspector] public List<ItemInstance> inventory = new List<ItemInstance>();
    [HideInInspector] public List<CatalogItem> catalog = new List<CatalogItem>();

    [Header("Events")]
    public UnityEvent onRefreshUI;

    // Singleton
    public static Trade instance;

    void Awake()
    {
        instance = this;
    }

    // Called from LoginRegister.onLoggedIn
    public void OnLoggedIn()
    {
        tradeCanvas.SetActive(true);

        if (onRefreshUI != null)
            onRefreshUI.Invoke();
    }

    // === INVENTORY ===
    public void GetInventory()
    {
        inventoryText.text = string.Empty;

        var request = new GetPlayerCombinedInfoRequest
        {
            PlayFabId = LoginRegister.instance.playFabId,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetUserInventory = true
            }
        };

        PlayFabClientAPI.GetPlayerCombinedInfo(
            request,
            result =>
            {
                inventory = result.InfoResultPayload.UserInventory;

                foreach (var item in inventory)
                {
                    inventoryText.text += item.DisplayName + ", ";
                }
            },
            error =>
            {
                SetDisplayText(error.ErrorMessage, true);
            });
    }

    // === CATALOG ===
    public void GetCatalog()
    {
        var request = new GetCatalogItemsRequest
        {
            CatalogVersion = "PlayerItems"   // Use your catalog version/name
        };

        PlayFabClientAPI.GetCatalogItems(
            request,
            result =>
            {
                catalog = result.Catalog;
            },
            error =>
            {
                SetDisplayText(error.ErrorMessage, true);
            });
    }

    // === Display text helper ===
    public void SetDisplayText(string text, bool isError)
    {
        if (displayText == null) return;

        displayText.text = text;
        displayText.color = isError ? Color.red : Color.green;

        CancelInvoke(nameof(HideDisplayText));
        Invoke(nameof(HideDisplayText), 2f);
    }

    void HideDisplayText()
    {
        if (displayText != null)
            displayText.text = string.Empty;
    }
}
