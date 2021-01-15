using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class Item
{
    public string name;
    public int id;
    public string description;
    public int mass;
    public Sprite icon;

    public Item (string name, int id, string description, int mass)
    {
        this.name = name;
        this.id = id;
        this.description = description;
        this.mass = mass;
        icon = Resources.Load<Sprite>("Items/Sprites/" + name);
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
