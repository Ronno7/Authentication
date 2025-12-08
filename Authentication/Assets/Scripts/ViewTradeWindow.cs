using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class ViewTradeWindow : MonoBehaviour
{
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI offeredItemsText;
    public TextMeshProUGUI requestedItemsText;

    TradeInfo currentTrade;

    public static ViewTradeWindow instance;

    void Awake()
    {
        instance = this;
    }

    public void SetTradeWindow(int tradeOfferIndex)
    {
        if (tradeOfferIndex < 0 || tradeOfferIndex >= TradeOffers.instance.tradeOffers.Count)
            return;

        currentTrade = TradeOffers.instance.tradeOffers[tradeOfferIndex];

        var offeredCount = new Dictionary<string, int>();
        var requestedCount = new Dictionary<string, int>();

        var displayName = TradeOffers.instance.tradeOfferInfo.playerDisplayNames[tradeOfferIndex];
        headerText.text = $"{displayName} wants to trade...";

        // Offered items
        foreach (var itemId in currentTrade.OfferedCatalogItemIds)
        {
            if (!offeredCount.ContainsKey(itemId))
                offeredCount[itemId] = 0;

            offeredCount[itemId]++;
        }

        offeredItemsText.text = "";
        foreach (var pair in offeredCount)
        {
            var catItem = Trade.instance.catalog.Find(c => c.ItemId == pair.Key);
            var name = catItem != null ? catItem.DisplayName : pair.Key;
            offeredItemsText.text += $"x{pair.Value} {name}\n";
        }

        // Requested items
        foreach (var itemId in currentTrade.RequestedCatalogItemIds)
        {
            if (!requestedCount.ContainsKey(itemId))
                requestedCount[itemId] = 0;

            requestedCount[itemId]++;
        }

        requestedItemsText.text = "";
        foreach (var pair in requestedCount)
        {
            var catItem = Trade.instance.catalog.Find(c => c.ItemId == pair.Key);
            var name = catItem != null ? catItem.DisplayName : pair.Key;
            requestedItemsText.text += $"x{pair.Value} {name}\n";
        }
    }

    public void OnAcceptTradeButton()
    {
        if (currentTrade == null)
            return;

        var inventoryItemsToSend = new List<string>();
        var tempInventory = new List<ItemInstance>(Trade.instance.inventory);

        // Make sure we have all requested items
        foreach (var requestedId in currentTrade.RequestedCatalogItemIds)
        {
            var invItem = tempInventory.Find(x => x.ItemId == requestedId);
            if (invItem == null)
            {
                Trade.instance.SetDisplayText(
                    "You don't have the requested items in your inventory.", true);
                return;
            }

            inventoryItemsToSend.Add(invItem.ItemInstanceId);
            tempInventory.Remove(invItem);
        }

        var request = new AcceptTradeRequest
        {
            TradeId = currentTrade.TradeId,
            OfferingPlayerId = currentTrade.OfferingPlayerId,
            AcceptedInventoryInstanceIds = inventoryItemsToSend
        };

        PlayFabClientAPI.AcceptTrade(
            request,
            result =>
            {
                RemoveTradeOwnerFromGroup(result.Trade.OfferingPlayerId);
            },
            error =>
            {
                Trade.instance.SetDisplayText(error.ErrorMessage, true);
            });
    }

    void RemoveTradeOwnerFromGroup(string offeringPlayerId)
    {
        var executeRequest = new ExecuteCloudScriptRequest
        {
            FunctionName = "AcceptTrade",
            FunctionParameter = new { tradeOwnerId = offeringPlayerId }
        };

        PlayFabClientAPI.ExecuteCloudScript(
            executeRequest,
            result =>
            {
                if (Trade.instance.onRefreshUI != null)
                    Trade.instance.onRefreshUI.Invoke();
            },
            error =>
            {
                Trade.instance.SetDisplayText(error.ErrorMessage, true);
            });
    }

    public void ResetUI()
    {
        headerText.text = "";
        offeredItemsText.text = "";
        requestedItemsText.text = "";
        currentTrade = null;
    }
}
