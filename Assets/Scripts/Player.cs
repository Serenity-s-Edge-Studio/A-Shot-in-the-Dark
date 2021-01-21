using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    private PlayerInput.PlayerActions input;
    [SerializeField]
    private Rigidbody2D rigidbody;
    public float speed = 1;
    [SerializeField]
    private LightDrop MuzzleFlash;
    [SerializeField]
    private LightDrop Bullet;

    private new Camera camera;
    [SerializeField]
    private float projectileSpeed;
    private Animator animator;
    [SerializeField]
    private Transform spawnPosition;
    private AudioSource source;
    [SerializeField]
    private AudioClip[] shootingClips;
    [SerializeField]
    private AudioClip[] pickupClips;
    [SerializeField]
    private AudioClip[] reloadClips;

    [SerializeField]
    private Image gunImage;
    [SerializeField]
    private TextMeshProUGUI ammoText;
    [SerializeField]
    private Sprite[] sprites;
    [SerializeField]
    private float fireRate;
    [SerializeField]
    private float spreadSize;
    private float currentFireRate = 0;

    private int ammo;
    private int clipSize = 7;
    private int currentMagazine;
    private Pickup.Type currentGunType = Pickup.Type.Machinegun;
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
    [SerializeField]
    private LightDrop glowStick;
    [SerializeField]
    private GameObject flameMuzzle;
    [SerializeField]
    private LightDrop flameProjectile;
    private bool isDead = false;

    public static Player instance;
    [SerializeField]
    private bool isInvincible;

    public PlayerInventoryController inventory;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        input = new PlayerInput().Player;
        input.Enable();
        input.Shoot.performed += Shoot_performed;
        input.DropWeapon.performed += DropWeapon_performed;
        input.Reload.performed += Reload_performed;
        camera = FindObjectOfType<Camera>();
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        inventory = GetComponent<PlayerInventoryController>();
        SwitchBackToPistol();
        updateUI();
    }

    private void Reload_performed(InputAction.CallbackContext obj)
    {
        StopAllCoroutines();
        Reload();
    }

    private void DropWeapon_performed(InputAction.CallbackContext obj)
    {
        SwitchBackToPistol();
    }

    private void Shoot_performed(InputAction.CallbackContext obj)
    {
        if (obj.ReadValueAsButton() && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            StartCoroutine(ShootCoroutine());
        else
        {
            StopAllCoroutines();
        }
    }
    private IEnumerator ShootCoroutine()
    {
        while (true)
        {
            if (currentFireRate < 0.01f)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();
                Vector2 worldPos = camera.ScreenToWorldPoint(mousePos);
                if (currentGunType != Pickup.Type.Flamethrower) Instantiate(MuzzleFlash, transform.position, Quaternion.identity);
                LightDrop bullet;
                float width;
                switch (currentGunType)
                {
                    case Pickup.Type.Flamethrower:
                        flameMuzzle.SetActive(true);
                        bullet = Instantiate(flameProjectile, spawnPosition.position, spawnPosition.rotation);
                        width = Random.Range(-1f, 1f) * (spreadSize + 60f);
                        bullet.rigidbody.AddForce(transform.right * projectileSpeed + transform.up * width);
                        break;
                    case Pickup.Type.Machinegun:
                        bullet = Instantiate(Bullet, spawnPosition.position, spawnPosition.rotation);
                        width = Random.Range(-1f, 1f) * (spreadSize + 10f);
                        bullet.rigidbody.AddForce(transform.right * projectileSpeed + transform.up * width);
                        bullet.GetComponentInChildren<Bullet>().damage = 4;
                        break;
                    case Pickup.Type.Shotgun:
                        for (int i = 0; i < 4; i++)
                        {
                            bullet = Instantiate(Bullet, spawnPosition.position, spawnPosition.rotation);
                            width = Random.Range(-1f, 1f) * (spreadSize + 45f);
                            bullet.rigidbody.AddForce(transform.right * projectileSpeed + transform.up * width);
                            bullet.GetComponentInChildren<Bullet>().damage = 3;
                        }
                        break;
                    case Pickup.Type.GlowstickLauncher:
                        bullet = Instantiate(glowStick, spawnPosition.position, spawnPosition.rotation);
                        width = Random.Range(-1f, 1f) * (spreadSize);
                        bullet.rigidbody.AddForce(transform.right * projectileSpeed + transform.up * width);
                        break;
                    case Pickup.Type.Pistol:
                        bullet = Instantiate(Bullet, spawnPosition.position, spawnPosition.rotation);
                        width = Random.Range(-1f, 1f) * (spreadSize);
                        bullet.rigidbody.AddForce(transform.right * projectileSpeed + transform.up * width);
                        bullet.GetComponentInChildren<Bullet>().damage = 5;
                        break;
                }
                
                animator.SetTrigger("Shoot");
                source.PlayOneShot(shootingClips[(int)currentGunType]);
                currentMagazine--;
                updateUI();
                if (currentMagazine == 0)
                {
                    if (!Reload())
                        SwitchBackToPistol();
                    yield return new WaitForSeconds(1f);
                }
                currentFireRate = fireRate;
            }
            yield return null;
        }
    }

    private void SwitchBackToPistol()
    {
        if (currentGunType != Pickup.Type.Pistol)
        {
            currentGunType = Pickup.Type.Pistol;
            clipSize = 7;
            fireRate = 60f / 45f;
            animator.SetBool("Pistol", true);
            Reload();
        }
    }

    private bool Reload()
    {
        animator.SetTrigger("Reload");
        source.PlayOneShot(reloadClips[(int)currentGunType]);
        if (currentGunType == Pickup.Type.Pistol)
        {
            currentMagazine = clipSize;
            return true;
        }
        else if (ammo > 0)
        {
            if (currentMagazine > 0)
            {
                ammo += currentMagazine;
                currentMagazine = 0;
                updateUI();
            }
            if (ammo < clipSize)
            {
                currentMagazine = ammo;
                ammo = 0;
            }
            else
            {
                ammo -= clipSize;
                currentMagazine = clipSize;
            }
            return true;
        }
        return false;
    }
    private void Update()
    {
        currentFireRate = Mathf.Max(0, currentFireRate - Time.deltaTime);
    }
    // Update is called once per frame
    void FixedUpdate()
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

    public void equipWeapon(Pickup.Type type, int ammo, int clipSize, int fireRate)
    {
        if (type != Pickup.Type.Pistol) animator.SetBool("Pistol", false);
        if (type == currentGunType)
        {
            this.ammo += ammo;
        }
        else
        {
            this.ammo = ammo - clipSize;
            currentMagazine = clipSize;
        }
        source.PlayOneShot(pickupClips[(int)type]);
        currentGunType = type;
        this.clipSize = clipSize;
        this.fireRate = 60f / fireRate;
        updateUI();
    }
    public void updateUI()
    {
        gunImage.sprite = sprites[(int)currentGunType];
        if (currentGunType == Pickup.Type.Pistol)
        {
            ammoText.text = $"{currentMagazine}/∞";
        }
        else
        {
            ammoText.text = $"{currentMagazine}/{ammo}";
        }
    }
    public void Damage(int amount)
    {
        if (!isInvincible)
        {
            health = Mathf.Max(0, health - amount);
            if (health == 0 && !isDead)
            {
                isDead = true;
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
}
