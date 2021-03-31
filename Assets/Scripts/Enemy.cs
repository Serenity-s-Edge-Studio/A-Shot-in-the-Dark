﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IIgnitable, IPooledObject
{
    public List<LightDrop> influencingLights = new List<LightDrop>();
    public Vector2 target;
    public float timeTillNextRandom;
    [SerializeField]
    private float StartingHealth;
    private float health;

    public new Rigidbody2D rigidbody;
    public Animator animator;
    public float attackCooldown;
    public GameObject deathParticles;
    public int AttackDamage;
    private bool isOnFire = false;

    public void OnObjectSpawn()
    {
        health = StartingHealth;
    }
    public void Damage(float amount)
    {
        health = Mathf.Max(0, health - amount);
        if (health < .01f)
        {
            if (ScoreManager.instance)
                ScoreManager.instance.scoreKill();
            EnemyManager.instance.DisableEnemy(this);
        }
        Destroy(Instantiate(deathParticles, transform.position, Quaternion.identity), 5);
    }

    public void SetFire(float dmgPerSecond)
    {
        if (!isOnFire)
        {
            isOnFire = true;
            EnemyManager.instance.activeEnemies.Remove(this);
            StartCoroutine(doDamagePerSecond(dmgPerSecond));
        }
    }

    private IEnumerator doDamagePerSecond(float dmgPerSecond)
    {
        while (health > 0.1f)
        {
            Damage(dmgPerSecond);
            rigidbody.AddTorque(1000);
            yield return new WaitForSeconds(1);
        }
    }
}
