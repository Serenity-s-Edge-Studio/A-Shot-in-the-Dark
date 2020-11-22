using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private PlayerInput.PlayerActions input;
    [SerializeField]
    private Rigidbody2D rigidbody;
    public float speed = 1;
    [SerializeField]
    private LightDrop LightPrefab;
    [SerializeField]
    private LightDrop MuzzleFlash;
    [SerializeField]
    private LightDrop Bullet;
    private new Camera camera;
    [SerializeField]
    private float projectileSpeed;
    private Animator animator;

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
    }

    private void Shoot_performed(InputAction.CallbackContext obj)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = camera.ScreenToWorldPoint(mousePos);
        Instantiate(MuzzleFlash, transform.position, Quaternion.identity);
        LightDrop bullet = Instantiate(Bullet, transform.position, Quaternion.identity);
        bullet.rigidbody.AddForce((worldPos - ((Vector2)transform.position)).normalized * projectileSpeed);
        animator.SetTrigger("Shoot");
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
}
