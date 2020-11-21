using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float damage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie") && collision.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.Damage(damage);
            Destroy(transform.parent.gameObject);
        }
    }
}
