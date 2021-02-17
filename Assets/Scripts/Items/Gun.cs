using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Items/Gun")]
public class Gun : Item, IEquipable
{
    public float FireRate;
    public float ReloadTime;
    public float DamageScale;
    public int ProjectileAmount;
    public float SpreadAngle;
    public string Caliber;
    public float MuzzleVelocity;
    public Ammo compatibleAmmo;
    public AudioClip ReloadClip;
    public AudioClip ShotClip;

    public void Equip(Entity entity)
    {
        entity.GetComponent<GunController>().EquipGun(this);
    }
}
