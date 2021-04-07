using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public List<ItemStack> items = new List<ItemStack>();
    public new Rigidbody2D rigidbody;
    public new CircleCollider2D collider;
    [SerializeField]
    private AudioClip _PickUpClip;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out Player player))
        {
            if (player.inventory.inventory.CanStoreAll(items))
            {
                foreach (ItemStack item in items)
                    player.inventory.inventory.TryAddItems(item.item, item.Amount);
                if (_PickUpClip != null)
                    player.GetComponent<AudioSource>().PlayOneShot(_PickUpClip, 10);
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
}
