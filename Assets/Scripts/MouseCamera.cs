using UnityEngine;
using UnityEngine.InputSystem;

public class MouseCamera : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform player;

    float xRotation = 0f;

    private void Update()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity;
        float mouseY = mouseDelta.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.Rotate(Vector3.up * mouseX);

    }

}
