using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class CreateTrade : MonoBehaviour
{
    public TradeItem[] offeringItems;
    public TradeItem[] requestingItems;

    public static CreateTrade instance;

    void Awake()
    {
        instance = this;
    }

    // Called by the Create Trade button
    public void OnCreateTradeButton()
    {
        if (Trade.instance == null || Trade.instance.inventory == null)
        {
            Trade.instance.SetDisplayText("Inventory not loaded.", true);
            return;
        }

        // Work on a copy of the inventory we can modify while selecting items
        var tempInventory = new List<ItemInstance>(Trade.instance.inventory);
        var itemsToOffer = new List<string>();

        // Collect offered items
        foreach (var item in offeringItems)
        {
            for (int i = 0; i < item.value; i++)
            {
                var invItem = tempInventory.Find(x => x.DisplayName == item.itemName);
                if (invItem == null)
                {
                    Trade.instance.SetDisplayText("You don't have enough items to offer.", true);
                    return;
                }

                itemsToOffer.Add(invItem.ItemInstanceId);
                tempInventory.Remove(invItem);
            }
        }

        if (itemsToOffer.Count == 0)
        {
            Trade.instance.SetDisplayText("You must offer at least one item.", true);
            return;
        }

        // Collect requested items (catalog item IDs)
        var itemsToRequest = new List<string>();

        foreach (var item in requestingItems)
        {
            if (item.value <= 0) continue;

            var catalogItem = Trade.instance.catalog.Find(c => c.DisplayName == item.itemName);
            if (catalogItem == null)
            {
                Trade.instance.SetDisplayText("Requested item not found in catalog.", true);
                return;
            }

            for (int i = 0; i < item.value; i++)
                itemsToRequest.Add(catalogItem.ItemId);
        }

        var request = new OpenTradeRequest
        {
            OfferedInventoryInstanceIds = itemsToOffer,
            RequestedCatalogItemIds = itemsToRequest
        };

        PlayFabClientAPI.OpenTrade(
            request,
            result =>
            {
                AddTradeToGroup(result.Trade.TradeId);
            },
            error =>
            {
                Trade.instance.SetDisplayText(error.ErrorMessage, true);
            });
    }

    void AddTradeToGroup(string tradeId)
    {
        var executeRequest = new ExecuteCloudScriptRequest
        {
            FunctionName = "AddNewTradeOffer",
            FunctionParameter = new { tradeID = tradeId }
        };

        PlayFabClientAPI.ExecuteCloudScript(
            executeRequest,
            result =>
            {
                Trade.instance.SetDisplayText("Trade offer created.", false);

                if (Trade.instance.onRefreshUI != null)
                    Trade.instance.onRefreshUI.Invoke();
            },
            error =>
            {
                Trade.instance.SetDisplayText(error.ErrorMessage, true);
            });
    }

    // Called by Trade.onRefreshUI
    public void ResetItemValues()
    {
        foreach (var item in offeringItems)
            item.ResetValue();

        foreach (var item in requestingItems)
            item.ResetValue();
    }
}
