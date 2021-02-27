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
    public bool CanRetrieveItems(Item item, in int amount, out int missingItems)
    {
        if (_ItemAmountDictionary.TryGetValue(item, out int inStorage))
        {
            missingItems = Mathf.Max(amount - inStorage, 0);
            return missingItems < 1;
        }
        missingItems = amount;
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

    public bool TryGetItem(Item item, out int amount)
    {
        return _ItemAmountDictionary.TryGetValue(item, out amount);
    }

    public bool CanStoreAll(List<ItemStack> stacks)
    {
        float mass = _CurrentMass;
        foreach (ItemStack stack in stacks)
        {
            mass += stack.item.mass * stack.Amount;
            if (mass > _Capacity) return false;
        }
        return true;
    }
    public bool ContainsAll(List<ItemStack> stacks)
    {
        return stacks.TrueForAll((stack) =>
        {
            return TryGetItem(stack.item, out int inStorage) && stack.Amount <= inStorage;
        });
    }
    public ItemStack[] GetSortedItems(SortingType sortMethod, bool isAscending)
    {
        List<ItemStack> stacks = new List<ItemStack>(_ItemAmountDictionary.Count);
        foreach (Item key in _ItemAmountDictionary.Keys)
        {
            if (_ItemAmountDictionary[key] > 0)
                stacks.Add(new ItemStack { item = key, Amount = _ItemAmountDictionary[key] });
        }
        stacks.Sort(new StackComparer<ItemStack>(sortMethod, isAscending));
        return stacks.ToArray();
    }

    public enum SortingType
    {
        Amount,
        Name,
    }
}
