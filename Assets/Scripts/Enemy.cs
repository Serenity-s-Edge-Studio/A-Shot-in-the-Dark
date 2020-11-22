using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public List<LightDrop> influencingLights = new List<LightDrop>();
    public Vector2 target;
    public float timeTillNextRandom;
    private float health;
    public Animator animator;
    public float attackCooldown;
    public GameObject deathParticles;

    public void Damage(float amount)
    {
        health = Mathf.Max(0, health - amount);
        if (health < .01f) Destroy(gameObject);
        Destroy(Instantiate(deathParticles, transform.position, Quaternion.identity), 5);
    }
    private void OnDestroy()
    {
        EnemyManager.instance.activeEnemies.Remove(this);
    }
}
