using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryController : MonoBehaviour
{
    public Inventory inventory;
    public virtual void Start()
    {
        inventory = GetComponentInChildren<Inventory>();
    }
    public abstract void DisplayInventory();
    public abstract void HideInventory();
}
