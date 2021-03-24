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

    private new Camera camera;
    private AudioSource source;

    [SerializeField]
    public bool toggleMovement;
    private int health = 100;
    [SerializeField]
    private Slider healthBar;
    [SerializeField]
    private GameObject deathText;
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

    public void Damage(int amount)
    {
        if (!isInvincible)
        {
            health = Mathf.Max(0, health - amount);
            if (health == 0 && !isDead)
            {
                isDead = true;
                gunController.CanShoot = false;
                deathText.SetActive(true);
                this.enabled = false;
                rigidbody.simulated = false;
                input.Disable();
                source.PlayOneShot(deathSound);
                Instantiate(deathParticles, transform.position, Quaternion.identity);
            }
            healthBar.value = health;
        }
    }
    public bool IsPositionInFOV(Vector2 position)
    {
        Vector2 screenPos = camera.WorldToScreenPoint(position);
        return Mathf.Abs(screenPos.x) > 1 || Mathf.Abs(screenPos.y) > 1;
    }
}
