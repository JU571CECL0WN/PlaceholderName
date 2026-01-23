using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnMove(InputValue value)
    {
        if (!IsOwner) return;

        moveInput = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * speed;
    }
}