using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryController : InventoryController
{
    [SerializeField]
    private GameObject _InventoryUI;
    [SerializeField]
    private GameObject _InventoryContainer;
    [SerializeField]
    private ItemSlot _ItemPrefab;
    [SerializeField]
    private ItemStack[] _StartingItems;

    private List<ItemSlot> _ItemList;
    private PlayerInput.PlayerActions input;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        _ItemList = new List<ItemSlot>();
        input = new PlayerInput().Player;
        input.Enable();
        input.Openinventory.performed += _ => 
        {
            if (_InventoryUI.activeInHierarchy) HideInventory();
            else
            {
                Player.instance.closeWindows();
                DisplayInventory();
            }
        };
        for (int i = 0; i < _StartingItems.Length; i++)
        {
            inventory.TryAddItems(_StartingItems[i].item, _StartingItems[i].Amount);
        }
    }

    public override void DisplayInventory()
    {
        _InventoryUI.SetActive(true);
        var sortedList = inventory.GetSortedItems(Inventory.SortingType.Amount, false);
        for (int i = 0; i < sortedList.Length; i++)
        {
            ItemStack stack = sortedList[i];
            if (i >= _ItemList.Count)
            {
                _ItemList.Add(Instantiate(_ItemPrefab, _InventoryContainer.transform));
            }
            _ItemList[i].name = stack.item.name;
            _ItemList[i].UpdateUI(stack, this);
            _ItemList[i].gameObject.SetActive(true);
        }
        for (int i = sortedList.Length; i < _ItemList.Count; i++)
        {
            _ItemList[i].gameObject.SetActive(false);
        }
    }

    public override void DropItem(ItemStack stack)
    {
        base.DropItem(stack);
        if(_InventoryUI.activeInHierarchy) DisplayInventory();
    }

    public override void HideInventory()
    {
        _InventoryUI.SetActive(false);
    }
    private void OnDestroy()
    {
        input.Disable();
    }
}
