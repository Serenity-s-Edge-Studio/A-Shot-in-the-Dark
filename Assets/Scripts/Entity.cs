using Assets.Scripts.Utility;
using UnityEngine;

public abstract class Entity : GridObject
{
    protected int health = 100;
    [SerializeField]
    protected int maxHealth;
    public abstract void Damage(int amount);
    public abstract void Heal(int amount);
}
