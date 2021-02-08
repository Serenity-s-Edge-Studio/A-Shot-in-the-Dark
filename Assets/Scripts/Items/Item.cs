using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class Item
{
    public static int ItemCount;
    public static Sprite Placeholder;
    public string name;
    public int id;
    public string description;
    public float mass;
    public Sprite icon;

    public Item (string name, string description, float mass)
    {
        this.name = name;
        this.id = ItemCount;
        ItemCount++;
        this.description = description;
        this.mass = mass;
        icon = Resources.Load<Sprite>("Items/Sprites/" + name); 
        if (icon == null)
        {
            if (Placeholder == null)
                Placeholder = Resources.Load<Sprite>("Items/Sprites/Placeholder");
            icon = Placeholder;
        }
    }
    public Item (Item item)
    {
        this.name = item.name;
        this.id = item.id;
        this.description = item.description;
        this.mass = item.mass;
        this.icon = item.icon;
    }
    public override int GetHashCode()
    {
        return id;
    }
    public override bool Equals(object obj)
    {
        if (obj is Item)
            return id == ((Item)obj).id;
        return false;
    }
}
