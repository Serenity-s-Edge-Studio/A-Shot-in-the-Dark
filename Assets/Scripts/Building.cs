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
    [SerializeField]
    private Collider2D _BuildAreaCollider;
    [SerializeField]
    private int currentHealth;
    [SerializeField]
    private GameObject DestructionEffect;
    public bool isBuilt;

    public UnityEvent OnBuilt;
    public UnityEvent OnDeath;
    private void Start()
    {
        if (isBuilt)
            Build();
    }
    public void Build()
    {
        isBuilt = true;
        currentHealth = _MaxHealth;
        OnBuilt.Invoke();
        _BuildAreaCollider.isTrigger = false;
        OnDeath.AddListener(() => 
            Instantiate(DestructionEffect, transform.position, transform.rotation * Quaternion.AngleAxis(-90f, Vector3.right)));
        BuildingManager.instance.Add(this);
    }

    public bool IsBuildAreaClear()
    {
        List<Collider2D> colliders = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Building"));
        return _BuildAreaCollider.OverlapCollider(filter, colliders) == 0;
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

    public override void Damage(int damage)
    {
        if (!isBuilt) return;
        currentHealth -= damage;
        if (currentHealth < 0.01f)
        {
            OnDeath.Invoke();
            BuildingManager.instance.Remove(this);
            Destroy(gameObject);
        }
    }
    public override void Heal(int amount)
    {
        throw new System.NotImplementedException();
    }
}
