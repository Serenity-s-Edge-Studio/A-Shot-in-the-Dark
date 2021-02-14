using TMPro;
using UnityEngine;
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
    [SerializeField]
    private Button _DropItemButton;
    [SerializeField]
    private Button _EquipItemButton;

    public void UpdateUI(IStackable<Item> item, InventoryController inventoryController)
    {
        Item value = item.getValue();
        icon.sprite = value.icon;
        nameText.text = $"Name: {value.name}";
        amountText.text = $"Amount: {item.TotalNumberOfItems}";
        massText.text = $"Mass: {item.TotalMass}";
        descriptionText.text = $"Decription: {value.description}";
        _DropItemButton.onClick.RemoveAllListeners();
        _DropItemButton.onClick.AddListener(() => inventoryController.DropItem(item));
        IEquipable equipable = item.getValue() as IEquipable;
        if (equipable != null)
        {
            _EquipItemButton.gameObject.SetActive(true);
            _EquipItemButton.onClick.RemoveAllListeners();
            _EquipItemButton.onClick.AddListener(() => equipable.Equip(inventoryController.entity));
        }
        else
        {
            _EquipItemButton.gameObject.SetActive(false);
        }
            
    }
}
