using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : Item
{
    public string caliber;
    public int damage;
    public Ammo(Ammo ammo) : base(ammo)
    {
        this.caliber = ammo.caliber;
        this.damage = ammo.damage;
    }

    public Ammo(string name, string description, float mass, string caliber, int damage) : base(name, description, mass)
    {
        this.caliber = caliber;
        this.damage = damage;
    }
}
