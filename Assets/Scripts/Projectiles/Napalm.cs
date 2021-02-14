using UnityEngine;

public class Napalm : Bullet
{
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IIgnitable>(out IIgnitable flamableObject))
        {
            GetComponentInParent<Rigidbody2D>().velocity = Vector2.zero;
            flamableObject.SetFire(damage);
        }
    }
}
