using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public List<ItemStack> items = new List<ItemStack>();
    public Rigidbody2D rigidbody;
    public CircleCollider2D collider;
    [SerializeField]
    private Type type;
    [SerializeField]
    private int max;
    [SerializeField]
    private int clipSize;
    [SerializeField]
    private int fireRate;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent<Player>(out Player player))
        {
            if (player.inventory.inventory.CanStoreAll(items))
            {
                foreach (ItemStack item in items)
                    player.inventory.inventory.TryAddItems(item.item, item.Amount);
                Destroy(gameObject);
            }
        }
    }

    public IEnumerator PreventCollision()
    {
        collider.enabled = false;
        while (rigidbody.velocity.magnitude > 0.1)
            yield return new WaitForFixedUpdate();
        collider.enabled = true;
    }

    public enum Type
    {
        Pistol,
        Shotgun,
        GlowstickLauncher,
        Machinegun,
        Flamethrower
    }
}
