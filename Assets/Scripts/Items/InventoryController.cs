using UnityEngine;

public abstract class InventoryController : MonoBehaviour
{
    [HideInInspector]
    public Entity entity;
    public Inventory inventory;
    
    [SerializeField]
    private Pickup _DropPrefab;
    [SerializeField]
    private float _EjectForce;


    public virtual void Start()
    {
        inventory = GetComponentInChildren<Inventory>();
        entity = GetComponent<Player>();
    }

    public virtual void DropItem(ItemStack stack)
    {
        int amount = stack.Amount;
        inventory.CanRetrieveItems(stack.item, in stack.Amount, out int missing);
        if (inventory.TryRetriveItems(stack.item, amount - missing))
        {
            Pickup droppedItem = Instantiate(_DropPrefab, gameObject.transform.position, Quaternion.identity);
            droppedItem.item = new ItemStack { item = stack.item, Amount = amount - missing };
            droppedItem.GetComponent<SpriteRenderer>().sprite = stack.item.icon;
            Vector2 force = Random.insideUnitCircle * _EjectForce;
            droppedItem.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
            droppedItem.StartCoroutine(droppedItem.PreventCollision());
        }
    }

    public abstract void DisplayInventory();

    public abstract void HideInventory();
}
