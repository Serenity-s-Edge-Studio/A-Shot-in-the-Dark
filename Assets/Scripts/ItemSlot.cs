using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI amountText;
    [SerializeField]
    private TextMeshProUGUI massText;
    [SerializeField]
    private TextMeshProUGUI descriptionText;

    public void UpdateUI(IStackable<Item> item)
    {
        Item value = item.getValue();
        icon.sprite = value.icon;
        nameText.text = $"Name: {value.name}";
        amountText.text = $"Amount: {item.TotalNumberOfItems}";
        massText.text = $"Mass: {item.TotalMass}";
        descriptionText.text = $"Decription: {value.description}";
    }
}
