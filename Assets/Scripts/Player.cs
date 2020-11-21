using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    PlayerInput.PlayerActions input;
    [SerializeField]
    Rigidbody2D rigidbody;
    public float speed = 1;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        input = new PlayerInput().Player;
        input.Enable();
    }

    private void Movement_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3)input.Movement.ReadValue<Vector2>().normalized * speed;
    }
}
