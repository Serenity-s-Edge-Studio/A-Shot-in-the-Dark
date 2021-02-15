using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Dictionary<int, IStackable<Item>> m_StackableInventory;
    private Dictionary<Item, int> _ItemAmountDictionary;
    [SerializeField]
    private float _Capacity;
    private float _CurrentMass;

    private void Start()
    {
        m_StackableInventory = new Dictionary<int, IStackable<Item>>();
        _ItemAmountDictionary = new Dictionary<Item, int>();
    }

    public bool CanStoreItems(Item item, in int amount, out int excess)
    {
        if (item.mass * amount <= _Capacity - _CurrentMass)
        {
            excess = 0;
            return true;
        }
        excess = amount - Mathf.RoundToInt((_Capacity - _CurrentMass) / item.mass);
        return false;
    }
    public bool TryAddItems(Item item, int amount) 
    {
        if (CanStoreItems(item, amount, out _))
        {
            _ItemAmountDictionary.TryGetValue(item, out int inStorage);
            _ItemAmountDictionary[item] = inStorage + amount;
            _CurrentMass += item.mass * amount;
            return true;
        }
        return false;
    }
    public bool CanRetrieveItems(Item item, in int amount, out int remaining)
    {
        if (_ItemAmountDictionary.TryGetValue(item, out int inStorage))
        {
            remaining = Mathf.Max(amount - inStorage, 0);
            return remaining < 1;
        }
        remaining = amount;
        return false;
    }
    /// <summary>
    /// Will only return true and remove the amount of items 
    /// if and only if the inventory can retrieve that amount of item.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool TryRetriveItems(Item item, int amount)
    {
        if (CanRetrieveItems(item, in amount, out int remaining))
        {
            _CurrentMass -= item.mass * (amount - remaining);
            _ItemAmountDictionary[item] -= (amount - remaining);
            return true;
        }
        return false;
    }

    public bool TryAddItems(IStackable<Item> stackable)
    {
        if (stackable.TotalMass > _Capacity - _CurrentMass) return false;
        if (m_StackableInventory.TryGetValue(stackable.getValue().id, out IStackable<Item> items))
        {
            if (!(items.getValue() == stackable.getValue()))
            {
                Debug.LogError("This inventory has encountered an Item with a duplicate key");
                return false;
            }
            items.Add(stackable.TotalNumberOfItems);
            return true;
        }
        m_StackableInventory.Add(stackable.getValue().id, stackable);
        return true;
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

    /// <summary>
    /// Tries to remove an amount of an item from the inventory adjusting the current mass in the process.
    /// </summary>
    /// <typeparam name="T">The item type to be removed.</typeparam>
    /// <param name="item">The item reference to be removed.</param>
    /// <param name="amount">The amount to remove.</param>
    /// <returns>Returns true if the item exists in the inventory. Sets the out value of amount to be the remaining count of items in the stack.</returns>
    private bool TryRemoveItems<T>(T item, ref int amount) where T : Item
    {
        if (m_StackableInventory.TryGetValue(item.id, out IStackable<Item> stack))
        {
            if (!(stack.getValue() is T))
            {
                Debug.LogError("This inventory has encountered an Item with a duplicate key");
                return false;
            }
            int tempAmount = stack.Remove(amount);
            if (tempAmount <= 0) m_StackableInventory.Remove(item.id);
            _CurrentMass -= tempAmount * item.mass;
            amount = tempAmount;
            return true;
        }
        return false;
    }
    public bool GetStack(Item item, out IStackable<Item> result)
    {
        return m_StackableInventory.TryGetValue(item.id, out result);
    }
    public bool TryAddItems(int id, int amount)
    {
        return ItemDatabase.instance.TryGetItem(id, out Item item) ? TryAddItems(item, amount) : false;
    }

    public bool TryAddItems(string name, int amount)
    {
        return ItemDatabase.instance.TryGetItem(name, out Item item) ? TryAddItems(item, amount) : false;
    }

    public bool TryRemoveItems(int id, ref int amount)
    {
        return ItemDatabase.instance.TryGetItem(id, out Item item) ? TryRemoveItems(item, ref amount) : false;
    }

    public bool TryRemoveItems(string name, int amount)
    {
        return ItemDatabase.instance.TryGetItem(name, out Item item) ? TryRemoveItems(item, ref amount) : false;
    }

    public IStackable<Item>[] GetSortedItems(SortingType sortMethod, bool isAscending)
    {
        List<IStackable<Item>> stacks = m_StackableInventory.Values.ToList();
        stacks.Sort(new StackComparer<Item>(sortMethod, isAscending));
        return stacks.ToArray();
    }

    public enum SortingType
    {
        Amount,
        Name,
    }
}
