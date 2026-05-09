using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private InputAction m_moveAction;
    private InputAction m_jumpAction;
    private InputAction m_talkAction;

    private Vector2 input;
    private Rigidbody rb;

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 14f;
    [SerializeField] private float accel = 80f;
    [SerializeField] private float jumpForce = 7f;

    static public bool dialogue = false;

    private void Awake()
    {
        m_moveAction = InputSystem.actions.FindAction("Move");
        m_jumpAction = InputSystem.actions.FindAction("Jump");
        m_talkAction = InputSystem.actions.FindAction("Interact");

        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (dialogue)
        {
            input = Vector2.zero;
            return;
        }
        else
            input = m_moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (dialogue)
        {
            rb.linearVelocity = Vector3.zero; // fully freeze movement
            return;
        }

        Vector3 moveDirection =
            transform.forward * input.y +
            transform.right * input.x; 

        moveDirection.Normalize();

        Vector3 targetVelocity = moveDirection * maxSpeed;

        Vector3 velocity = rb.linearVelocity;

        Vector3 velocityChange = targetVelocity - new Vector3(velocity.x, 0, velocity.z);

        velocityChange = Vector3.ClampMagnitude(velocityChange, accel * Time.fixedDeltaTime);
        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        if (m_jumpAction.WasPressedThisFrame())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}