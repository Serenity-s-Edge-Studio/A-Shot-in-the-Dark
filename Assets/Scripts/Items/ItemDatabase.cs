using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase instance;
    public int TotalItems;
    public ItemStackDefinition[] itemStackDefinitions;
    private Dictionary<int, Item> ItemIDDictionary;
    private Dictionary<string, int> NameIDDictionary;
    private Dictionary<int, int> StackSizeDictionary;

    private void Awake()
    {
        if (instance != null) Destroy(instance);
        instance = this;

        ItemIDDictionary = new Dictionary<int, Item>(itemStackDefinitions.Length);
        NameIDDictionary = new Dictionary<string, int>(itemStackDefinitions.Length);
        StackSizeDictionary = new Dictionary<int, int>(itemStackDefinitions.Length);
        foreach (ItemStackDefinition entry in itemStackDefinitions)
        {
            Item item = entry.item;
            ItemIDDictionary.Add(item.id, item);
            NameIDDictionary.Add(item.name, item.id);
            StackSizeDictionary.Add(item.id, entry.StackSize);
            TotalItems++;
        }
    }

    public bool TryGetItem(int id, out Item item)
    {
        if (ItemIDDictionary.TryGetValue(id, out item))
            return true;
        Debug.LogWarning($"The item could not be found because no Item exists with an ID of {id}");
        return false;
    }

    public bool TryGetItem(string name, out Item item)
    {
        int id = -1;
        if (NameIDDictionary.TryGetValue(name, out id))
        {
            return ItemIDDictionary.TryGetValue(id, out item);
        }
        Debug.LogWarning($"The item could not be found because no ItemID exists for the name: {name}");
        item = null;
        return false;
    }

    public bool TryGetStackSize(int id, out int size)
    {
        return StackSizeDictionary.TryGetValue(id, out size);
    }
}
[Serializable]
public class ItemStackDefinition
{
    public Item item;
    public int StackSize;
}
