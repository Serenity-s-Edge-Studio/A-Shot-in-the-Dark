using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Building : Entity
{
    [SerializeField]
    private ItemStack[] _Costs;
    [SerializeField]
    private int _MaxHealth;

    private int currentHealth;
    public bool isBuilt;

    public UnityEvent OnBuilt;
    public UnityEvent OnDeath;

    public void Build()
    {
        isBuilt = true;
        currentHealth = _MaxHealth;
        OnBuilt.Invoke();
    }

    public ItemStack[] GetRequiredItems()
    {
        float percentageMissing = 1 - (currentHealth / _MaxHealth);
        ItemStack[] requiredItems = new ItemStack[_Costs.Length];
        for (int i = 0; i < _Costs.Length; i++)
        {
            requiredItems[i] = new ItemStack { 
                item = _Costs[i].item, 
                Amount = Mathf.RoundToInt(_Costs[i].Amount * percentageMissing) 
            };
        }
        return requiredItems;
    }

    public void Damage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0.01f)
        {
            OnDeath.Invoke();
            Destroy(gameObject);
        }
    }
}
