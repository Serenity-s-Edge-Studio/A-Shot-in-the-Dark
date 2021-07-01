using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    public float damage;

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie") && collision.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.Damage(damage);
            Destroy(transform.parent.gameObject, 0.2f);
            DifficultyAdjuster.instance.scoreHit();
            gameObject.SetActive(false);
        }
    }
}
