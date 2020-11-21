using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float damage;
    [SerializeField]
    private int penetration;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie") && collision.TryGetComponent<Enemy>(out Enemy enemy))
        {
            penetration--;
            if (penetration > 0)
            enemy.Damage(damage);
            if (penetration <= 0)
            {
                Destroy(transform.parent.gameObject, 0.2f);
                this.enabled = false;
            }
        }
    }
}
