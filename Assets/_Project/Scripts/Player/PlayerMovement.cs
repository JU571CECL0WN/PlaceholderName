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

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            rb.simulated = false; 
            return;
        }
        
        Camera.main.GetComponent<CameraFollow>().target = transform;
    }

    void OnMove(InputValue value)
    {
        if (!IsOwner) return;
        moveInput = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        if (!IsOwner) return; 

        if (GetComponent<PlayerState>().isSleeping.Value)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = moveInput * speed;
    }

    void OnAction()
    {
        if (!IsOwner) return;

        var state = GetComponent<PlayerState>();
        if (state.isSleeping.Value)
        {
            state.RequestWakeUpServerRpc();
            return;
        }

        return;

    }
}