using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Items/Gun")]
public class Gun : Item, IEquipable
{
    public  float FireRate;
    public  float DamageScale;
    public  int ProjectileAmount;
    public  float SpreadAngle;
    public  string Caliber;
    public  float MuzzleVelocity;
    public  Ammo compatibleAmmo;
    public  AudioClip ReloadClip;
    public  AudioClip ShotClip;

    public void Equip(Entity entity)
    {
        entity.GetComponent<GunController>().EquipGun(this);
    }

    //public Gun(string name, string description, float mass, float fireRate, float damageScale, string caliber, float spreadAngle, int projectileAmount)
    //    : base(name, description, mass)
    //{
    //    this.FireRate = fireRate;
    //    this.DamageScale = damageScale;
    //    this.Caliber = caliber;
    //    this.ProjectileAmount = projectileAmount;
    //    this.SpreadAngle = spreadAngle;
    //}
}
