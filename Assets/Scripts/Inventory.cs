using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Inventory : MonoBehaviour
{
    private Dictionary<int, IStackable<Item>> m_StackableInventory;
    private Dictionary<int, List<Item>> m_UniqueItems;
    [SerializeField]
    private int _Capacity;
    private int _CurrentMass;

    private void Start()
    {
        m_StackableInventory = new Dictionary<int, IStackable<Item>>();
        m_UniqueItems = new Dictionary<int, List<Item>>();
    }
    private bool TryAddItems<T>(T item, int amount) where T : Item
    {
        if (item.mass * amount > _Capacity - _CurrentMass) return false;
        if (m_StackableInventory.TryGetValue(item.id, out IStackable<Item> items))
        {
            if (!(items.getValue() is T))
            {
                Debug.LogError("This inventory has encountered an Item with a duplicate key");
                return false;
            }
            items.Add(amount);
            return true;
        }
        int stackSize = ItemDatabase.instance.TryGetStackSize(item.id, out int size) ? size : 64;
        m_StackableInventory.Add(item.id, new Stackable<T>(item, amount, stackSize));
        return true;
    }
    public bool TryAddItems(int id, int amount)
    {
        return ItemDatabase.instance.TryGetItem(id, out Item item) ? TryAddItems(item, amount) : false;
    }
    public bool TryAddItems(string name, int amount)
    {
        return ItemDatabase.instance.TryGetItem(name, out Item item) ? TryAddItems(item, amount) : false;
    }
    public IStackable<Item>[] GetSortedItems(SortingType sorting, bool ascending)
    {
        List<IStackable<Item>> stacks = m_StackableInventory.Values.ToList();
        stacks.Sort(new StackComparer<Item>(sorting, ascending));
        return stacks.ToArray();
    }
    public enum SortingType
    {
        Amount,
        Name,
    }
}
