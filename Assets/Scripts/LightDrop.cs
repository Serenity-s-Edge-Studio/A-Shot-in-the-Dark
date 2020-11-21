using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightDrop : MonoBehaviour
{
    public Rigidbody2D rigidbody;
    [SerializeField]
    private Light2D light;
    [SerializeField]
    private bool Decay;
    [SerializeField]
    private float decayTime;
    [SerializeField]
    private float decaySpeed;
    private float remainingTime;

    private float startingColliderRadius;
    private float startingLightRadius;
    private CircleCollider2D collider;
    private List<Enemy> enemiesInRange = new List<Enemy>();

    private void Start()
    {
        remainingTime = decayTime;
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<CircleCollider2D>();
        light = GetComponent<Light2D>();
        startingColliderRadius = collider.radius;
        startingLightRadius = light.pointLightOuterRadius;
    }
    // Update is called once per frame
    void Update()
    {
        if (Decay)
        {
            remainingTime = Mathf.Max(0, remainingTime - Time.deltaTime * decaySpeed);
            float remaining = remainingTime / decayTime;
            if (remainingTime < .01f) Destroy(gameObject);
            light.pointLightOuterRadius = startingLightRadius * remaining;
            collider.radius = startingColliderRadius * remaining;
            //light.intensity = remaining;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemiesInRange.Add(enemy);
            enemy.influencingLights.Add(this);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemiesInRange.Remove(enemy);
            enemy.influencingLights.Remove(this);
        }
    }
    private void OnDestroy()
    {
        foreach(Enemy enemy in enemiesInRange)
        {
            enemy.influencingLights.Remove(this);
        }
    }
}
