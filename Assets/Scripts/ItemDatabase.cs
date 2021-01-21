using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase instance;
    public int TotalItems;
    private Dictionary<int, Item> ItemIDDictionary;
    private Dictionary<string, int> NameIDDictionary;
    private Dictionary<int, int> StackSizeDictionary;

    private void Awake()
    {
        if (instance != null) Destroy(instance);
        instance = this;
        List<(Item item, int? size)> items = new List<(Item item, int? size)>() 
        {
            (new Item("Gun", 0, "Pew pew", 10), 64),
            (new Item("Stick", 1, "Smack smack", 1), 32),
            (new Item("Wood", 2, "It's woody", 5), 128),
        };

        ItemIDDictionary = new Dictionary<int, Item>(items.Count);
        NameIDDictionary = new Dictionary<string, int>(items.Count);
        StackSizeDictionary = new Dictionary<int, int>(items.Count);
        foreach ((Item item, int? size) entry in items) {
            Item item = entry.item;
            ItemIDDictionary.Add(item.id, item);
            NameIDDictionary.Add(item.name, item.id);
            if (entry.size != null)
                StackSizeDictionary.Add(item.id, (int)entry.size);
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
        if (NameIDDictionary.TryGetValue(name, out id)) {
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
