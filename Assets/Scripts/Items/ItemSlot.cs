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
    private ItemDescription _DescriptionPrefab;
    [SerializeField]
    private Button _DropItemButton;
    [SerializeField]
    private Button _EquipItemButton;
    [SerializeField]
    private Button _ConsumeItemButton;
    [SerializeField]
    private Button _ViewDescriptionButton;

    public void UpdateUI(ItemStack stack, InventoryController inventoryController)
    {
        Item value = stack.item;
        icon.sprite = value.icon;
        nameText.text = $"Name: {value.name}";
        amountText.text = $"Amount: {stack.Amount}";
        massText.text = $"Mass: {stack.item.mass * stack.Amount}";
        if (descriptionText == null)
        {
            descriptionText = Instantiate(_DescriptionPrefab, transform.parent).text;
            GameObject parentGO = descriptionText.transform.parent.gameObject;
            _ViewDescriptionButton.onClick.AddListener(() =>
            {
                _ViewDescriptionButton.GetComponent<Animator>().SetTrigger("Flip");
                parentGO.SetActive(!parentGO.activeInHierarchy);
            });
            parentGO.SetActive(false);
        }
        descriptionText.text = value.description;
        _DropItemButton.onClick.RemoveAllListeners();
        _DropItemButton.onClick.AddListener(() => inventoryController.DropItem(stack));
        EnableContextualActionButtons(stack, inventoryController);

    }

    private void EnableContextualActionButtons(ItemStack stack, InventoryController inventoryController)
    {
        if (stack.item is IEquipable equipable)
        {
            _EquipItemButton.gameObject.SetActive(true);
            _EquipItemButton.onClick.RemoveAllListeners();
            _EquipItemButton.onClick.AddListener(() =>
            {
                equipable.Equip(inventoryController.entity);
                inventoryController.DisplayInventory();
            });
        }
        else
        {
            _EquipItemButton.gameObject.SetActive(false);
        }
        if (stack.item is IConsumable consumable)
        {
            _ConsumeItemButton.gameObject.SetActive(true);
            _ConsumeItemButton.onClick.RemoveAllListeners();
            _ConsumeItemButton.onClick.AddListener(() =>
            {
                consumable.Consume(inventoryController.entity);
                inventoryController.DisplayInventory();
            });
        }
        else
        {
            _ConsumeItemButton.gameObject.SetActive(false);
        }
    }
}
