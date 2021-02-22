using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingCostUI : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI amountText;
    [SerializeField]
    private TextMeshProUGUI availableText;
    public void UpdateUI(ItemStack stack, InventoryController inventory)
    {
        Item item = stack.item;
        icon.sprite = item.icon;
        nameText.text = $"{item.name}";
        amountText.text = $"Required: {stack.Amount}";
        availableText.text = $"Available: {(inventory.inventory.TryGetItem(item, out int inStorage) ? inStorage : 0)}";
    }
}
