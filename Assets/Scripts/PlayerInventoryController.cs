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
        }
        for (int i = sortedList.Length; i < _ItemList.Count; i++)
        {
            _ItemList[i].gameObject.SetActive(false);
        }
    }
    public override void HideInventory()
    {
        _InventoryUI.SetActive(false);
    }
}
