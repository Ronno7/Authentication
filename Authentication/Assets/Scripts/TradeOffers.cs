using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TradeOfferInfo
{
    public List<string> playerIds;
    public List<string> playerDisplayNames;
    public List<string> tradeIds;
}

public class TradeOffers : MonoBehaviour
{
    public Button[] tradeOfferButtons;

    [HideInInspector] public List<TradeInfo> tradeOffers = new List<TradeInfo>();
    [HideInInspector] public TradeOfferInfo tradeOfferInfo;

    int numTradeOffers;

    public static TradeOffers instance;

    void Awake()
    {
        instance = this;
    }

    // Called by Trade.onRefreshUI
    public void UpdateTradeOffers()
    {
        DisableAllTradeOfferButtons();

        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetTradeIDs"
        };

        PlayFabClientAPI.ExecuteCloudScript(
            request,
            result =>
            {
                var raw = result.FunctionResult.ToString();
                tradeOfferInfo = JsonUtility.FromJson<TradeOfferInfo>(raw);
                GetTradeInfo();
            },
            error =>
            {
                Trade.instance.SetDisplayText(error.ErrorMessage, true);
            });
    }

    void DisableAllTradeOfferButtons()
    {
        foreach (var button in tradeOfferButtons)
            button.gameObject.SetActive(false);
    }

    void GetTradeInfo()
    {
        tradeOffers = new List<TradeInfo>();

        if (tradeOfferInfo == null || tradeOfferInfo.playerIds == null)
        {
            UpdateTradeOffersUI();
            return;
        }

        numTradeOffers = tradeOfferInfo.playerIds.Count;

        if (numTradeOffers == 0)
        {
            UpdateTradeOffersUI();
            return;
        }

        for (int i = 0; i < tradeOfferInfo.playerIds.Count; i++)
        {
            var statusRequest = new GetTradeStatusRequest
            {
                OfferingPlayerId = tradeOfferInfo.playerIds[i],
                TradeId = tradeOfferInfo.tradeIds[i]
            };

            PlayFabClientAPI.GetTradeStatus(
                statusRequest,
                result =>
                {
                    tradeOffers.Add(result.Trade);

                    if (tradeOffers.Count == numTradeOffers)
                        UpdateTradeOffersUI();
                },
                error =>
                {
                    Trade.instance.SetDisplayText(error.ErrorMessage, true);
                });
        }
    }

    void UpdateTradeOffersUI()
    {
        for (int i = 0; i < tradeOfferButtons.Length; i++)
        {
            bool hasOffer = i < tradeOffers.Count;
            var btn = tradeOfferButtons[i];

            btn.gameObject.SetActive(hasOffer);
            if (!hasOffer) continue;

            btn.onClick.RemoveAllListeners();
            int index = i;
            btn.onClick.AddListener(() => OnTradeOfferButton(index));

            var label = btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            label.text = tradeOfferInfo.playerDisplayNames[i];
        }
    }

    public void OnTradeOfferButton(int tradeIndex)
    {
        ViewTradeWindow.instance.SetTradeWindow(tradeIndex);
    }
}
