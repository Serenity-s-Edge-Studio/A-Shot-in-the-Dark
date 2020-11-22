using System;
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

    [SerializeField]
    private Image gunImage;
    [SerializeField]
    private TextMeshProUGUI ammoText;
    [SerializeField]
    private Sprite[] sprites;
    [SerializeField]
    private float fireRate;
    private float currentFireRate = 0;

    private int ammo;
    private int clipSize = 7;
    private int currentMagazine;
    private Pickup.Type equippedGun = Pickup.Type.Pistol;

    public static Player instance;
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
        camera = FindObjectOfType<Camera>();
        animator = GetComponent<Animator>();
        SwitchBackToPistol();
        updateUI();
    }


    private void Shoot_performed(InputAction.CallbackContext obj)
    {
        if (obj.ReadValueAsButton())
            StartCoroutine(ShootCoroutine());
        else
            StopAllCoroutines();
    }
    private IEnumerator ShootCoroutine()
    {
        while (true)
        {
            if (currentFireRate < 0.01f)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();
                Vector2 worldPos = camera.ScreenToWorldPoint(mousePos);
                Instantiate(MuzzleFlash, transform.position, Quaternion.identity);
                LightDrop bullet = Instantiate(Bullet, spawnPosition.position, Quaternion.identity);
                bullet.rigidbody.AddForce((worldPos - ((Vector2)transform.position)).normalized * projectileSpeed);
                animator.SetTrigger("Shoot");
                currentMagazine--;
                updateUI();
                if (currentMagazine == 0)
                {
                    if (!Reload())
                        SwitchBackToPistol();
                    updateUI();
                    yield return new WaitForSeconds(1f);
                }
            }
            yield return null;
        }
    }

    private void SwitchBackToPistol()
    {
        equippedGun = Pickup.Type.Pistol;
        clipSize = 7;
        fireRate = 60f/45f;
        Reload();
    }

    private bool Reload()
    {
        animator.SetTrigger("Reload");
        if (equippedGun == Pickup.Type.Pistol)
        {
            currentMagazine = clipSize;
            return true;
        }
        else if (ammo > 0)
        {
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

        transform.position += ((-transform.up * axis.x) + (transform.right * axis.y)).normalized * speed;
    }

    public void equipWeapon(Pickup.Type type, int ammo, int clipSize, int fireRate)
    {
        equippedGun = type;
        this.clipSize = clipSize;
        currentMagazine = clipSize;
        this.ammo = ammo - clipSize;
        this.fireRate = 60f / fireRate;
        updateUI();
    }
    public void updateUI()
    {
        gunImage.sprite = sprites[(int)equippedGun];
        if (equippedGun == Pickup.Type.Pistol)
        {
            ammoText.text = $"{currentMagazine}/∞";
        }
        else
        {
            ammoText.text = $"{currentMagazine}/{ammo}";
        }
    }
}
