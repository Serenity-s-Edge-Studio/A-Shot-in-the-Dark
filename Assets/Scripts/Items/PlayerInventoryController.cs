using System.Collections;
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
    private List<ItemSlot> _ItemList;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        var input = new PlayerInput().Player;
        input.Enable();
        input.Openinventory.performed += _ => { if (_InventoryUI.activeInHierarchy) HideInventory(); else DisplayInventory(); };
        inventory.TryAddItems(0, 1);
        inventory.TryAddItems(1, 300);
        inventory.TryAddItems("Wood", 10);
    }
    public override void DisplayInventory()
    {
        _InventoryUI.SetActive(true);
        var sortedList = inventory.GetSortedItems(Inventory.SortingType.Amount, false);
        for (int i = 0; i < sortedList.Length; i++)
        {
            IStackable<Item> item = sortedList[i];
            Debug.Log($"{item.getValue().name} {item.TotalNumberOfItems}");
            if (i >= _ItemList.Count)
            {
                _ItemList.Add(Instantiate(_ItemPrefab, _InventoryContainer.transform));
            }
            _ItemList[i].name = item.getValue().name;
            _ItemList[i].UpdateUI(item, this);
            _ItemList[i].gameObject.SetActive(true);
        }
        for (int i = sortedList.Length; i < _ItemList.Count; i++)
        {
            _ItemList[i].gameObject.SetActive(false);
        }
    }
    public override void DropItem(IStackable<Item> stack)
    {
        base.DropItem(stack);
        DisplayInventory();
    }
    public override void HideInventory()
    {
        _InventoryUI.SetActive(false);
    }
}
