using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private InputAction MoveAction;
    private InputAction TalkAction;

    private Vector2 input;
    private Rigidbody rb;

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 14f;
    [SerializeField] private float accel = 80f;
    [SerializeField] private Transform cameraTransform;

    static public bool dialogue = false;

    private void Awake()
    {
        MoveAction = InputSystem.actions.FindAction("Move");
        TalkAction = InputSystem.actions.FindAction("Interact");

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
            input = MoveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (dialogue)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection =
            forward * input.y +
            right * input.x;  

        Vector3 targetVelocity = moveDirection * maxSpeed;

        Vector3 velocity = rb.linearVelocity;

        Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);

        Vector3 velocityChange = targetVelocity - horizontalVelocity;

        velocityChange = Vector3.ClampMagnitude(
            velocityChange,
            accel * Time.fixedDeltaTime
        );

        rb.AddForce(
            new Vector3(velocityChange.x, 0f, velocityChange.z),
            ForceMode.VelocityChange
        );
    }
}