using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : Entity
{
    private PlayerInput.PlayerActions input;
    [SerializeField]
    private new Rigidbody2D rigidbody;
    public float speed = 1;
    [SerializeField]
    private new Camera camera;
    private AudioSource source;

    [SerializeField]
    public bool toggleMovement;
    [SerializeField]
    private Slider healthBar;
    [SerializeField]
    private GameObject deathContainer;
    [SerializeField]
    private Button respawnButton;
    [SerializeField]
    private TextMeshProUGUI campfireCount;
    [SerializeField]
    private GameObject deathParticles;
    [SerializeField]
    private AudioClip deathSound;

    private bool isDead = false;

    public static Player instance;
    [SerializeField]
    private bool isInvincible;

    public PlayerInventoryController inventory;
    private PlayerGunController gunController;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        input = new PlayerInput().Player;
        input.Enable();
        camera = FindObjectOfType<Camera>();
        source = GetComponent<AudioSource>();
        inventory = GetComponent<PlayerInventoryController>();
        gunController = GetComponent<PlayerGunController>();
        health = maxHealth;
        healthBar.maxValue = maxHealth;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = camera.ScreenToWorldPoint(mousePos);
        Vector2 dir = worldPos - (Vector2)transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Vector2 axis = input.Movement.ReadValue<Vector2>().normalized;
        if (toggleMovement) transform.position += (Vector3)axis * speed;
        else transform.position += ((-transform.up * axis.x) + (transform.right * axis.y)).normalized * speed;
    }

    public override void Damage(int amount)
    {
        if (!isInvincible)
        {
            health = Mathf.Max(0, health - amount);
            if (health == 0 && !isDead)
            {
                Die();
            }
            healthBar.value = health;
        }
    }

    private void Die()
    {
        isDead = true;
        gunController.CanShoot = false;

        deathContainer.SetActive(true);
        int avaliableCampfires = BuildingManager.instance.SpawnPoints.Count;
        respawnButton.interactable = avaliableCampfires > 0;
        campfireCount.text = $"Avaliable campfires: {avaliableCampfires}";
        respawnButton.onClick.RemoveAllListeners();
        respawnButton.onClick.AddListener(Respawn);
        inventory.DropAllItems(.5f);

        this.enabled = false;
        rigidbody.simulated = false;
        input.Disable();
        source.PlayOneShot(deathSound);
        Instantiate(deathParticles, transform.position, Quaternion.identity);
    }

    private void Respawn()
    {
        isDead = false;
        gunController.CanShoot = true;

        deathContainer.SetActive(false);

        this.enabled = true;
        rigidbody.simulated = true;
        input.Enable();
        Heal(maxHealth);

        transform.position = BuildingManager.instance.GetClosestRespawnPoint(transform.position);
    }

    public override void Heal(int amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        healthBar.value = health;
    }
    public bool IsPositionInFOV(Vector2 position)
    {
        Vector2 screenPos = camera.WorldToViewportPoint(position);
        return screenPos.x < 1f && screenPos.x > 0f && screenPos.y < 1f && screenPos.y > 0f;
    }
}
