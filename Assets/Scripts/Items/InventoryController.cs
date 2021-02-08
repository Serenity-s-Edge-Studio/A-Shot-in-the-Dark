using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryController : MonoBehaviour
{
    public Inventory inventory;
    [SerializeField]
    private Pickup _DropPrefab;
    [SerializeField]
    private float _EjectForce;
    public virtual void Start()
    {
        inventory = GetComponentInChildren<Inventory>();
    }
    public virtual void DropItem(IStackable<Item> stack)
    {
        int amount = stack.TotalNumberOfItems;
        int missing = amount;
        if (inventory.TryRemoveItems(stack.getValue().id, ref missing))
        {
            Pickup droppedItem = Instantiate(_DropPrefab, gameObject.transform.position, Quaternion.identity);
            droppedItem.item = new Stackable<Item>(stack, amount - missing);
            droppedItem.GetComponent<SpriteRenderer>().sprite = stack.getValue().icon;
            Vector2 force = Random.insideUnitCircle * _EjectForce;
            droppedItem.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
            droppedItem.StartCoroutine(droppedItem.PreventCollision());
        }
    }
    public abstract void DisplayInventory();
    public abstract void HideInventory();
}
