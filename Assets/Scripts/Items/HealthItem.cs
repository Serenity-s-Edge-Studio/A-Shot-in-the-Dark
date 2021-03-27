using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Health Item", menuName = "Items/Health Item")]
public class HealthItem : Item, IConsumable
{
    public int RestoredHealth;
    public void Consume(Entity entity)
    {
        entity.Heal(RestoredHealth);
        if (entity.TryGetComponent(out InventoryController inventoryController))
        {
            inventoryController.inventory.TryRetriveItems(this, 1);
        }
    }
}
