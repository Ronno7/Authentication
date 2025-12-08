using TMPro;
using UnityEngine;

public class TradeItem : MonoBehaviour
{
    public string itemName;
    public int value;
    public TextMeshProUGUI displayText;

    void Start()
    {
        UpdateDisplayText();
    }

    void UpdateDisplayText()
    {
        if (displayText != null)
            displayText.text = $"{itemName} ({value})";
    }

    public void OnAddItemButton()
    {
        value++;
        UpdateDisplayText();
    }

    public void OnRemoveItemButton()
    {
        if (value > 0)
        {
            value--;
            UpdateDisplayText();
        }
    }

    public void ResetValue()
    {
        value = 0;
        UpdateDisplayText();
    }
}
