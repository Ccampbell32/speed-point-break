using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f; // Movement speed
    public float mouseSensitivity = 2f; // Mouse sensitivity for looking around
    public Transform playerCamera; // Reference to the player's camera
    public float lookUpLimit = 80f; // Limit for looking up
    public float lookDownLimit = 80f; // Limit for looking down

    private float rotationX = 0f; // Rotation around the X axis

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
    }

    void Update()
    {
        // Handle mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookDownLimit, lookUpLimit);
        playerCamera.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Handle movement
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxis("Vertical"); // W/S or Up/Down

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 movement = moveDirection * speed * Time.deltaTime;
        Vector3 newPosition = transform.position + transform.TransformDirection(movement);

        // Move the player
        transform.position = newPosition;
    }
}
