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
    private LightDrop Bullet;
    private Camera camera;
    [SerializeField]
    private float projectileSpeed;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        input = new PlayerInput().Player;
        input.Enable();
        input.Shoot.performed += Shoot_performed;
        camera = FindObjectOfType<Camera>();
    }

    private void Shoot_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = camera.ScreenToWorldPoint(mousePos);
        Instantiate(LightPrefab, transform.position, Quaternion.identity);
        LightDrop bullet = Instantiate(Bullet, transform.position, Quaternion.identity);
        bullet.rigidbody.AddForce((worldPos - ((Vector2)transform.position)).normalized * projectileSpeed);
    }

    private void Movement_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += (Vector3)input.Movement.ReadValue<Vector2>().normalized * speed;
    }
}
