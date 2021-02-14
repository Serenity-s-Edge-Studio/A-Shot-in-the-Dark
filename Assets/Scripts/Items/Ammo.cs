using UnityEngine;
[CreateAssetMenu(fileName = "Ammo", menuName = "Items/Ammo")]
public class Ammo : Item
{
    private static LightDrop BulletPlaceholder;

    public string caliber;
    public int damage;
    public LightDrop BulletPrefab;

    //public Ammo(string name, string description, float mass, string caliber, int damage, string bulletName)
    //    : base(name, description, mass)
    //{
    //    this.caliber = caliber;
    //    this.damage = damage;
    //    this.BulletPrefab = Resources.Load<LightDrop>("Items/Projectiles/" + bulletName);
    //    if (BulletPrefab == null)
    //    {
    //        if (BulletPlaceholder == null)
    //            BulletPlaceholder = Resources.Load<LightDrop>("Items/Projectiles/Bullet");
    //        this.BulletPrefab = BulletPlaceholder;
    //    }
    //}
}
