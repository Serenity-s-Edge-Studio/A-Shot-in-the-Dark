using System.Collections;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public Stackable<Item> item;
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
            if (item != null)
            {
                if (!player.inventory.inventory.TryAddItems(item.getValue().id, item.TotalNumberOfItems)) return;
            }
            else
                player.equipWeapon(type, Random.Range(clipSize, max), clipSize, fireRate);
            Destroy(gameObject);
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
