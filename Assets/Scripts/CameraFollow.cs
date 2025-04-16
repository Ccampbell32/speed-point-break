using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float distance = 5f;
    public float height = 3f;
    public float smoothSpeed = 0.125f;
    public float mouseSensitivity = 5f; // Adjust this for mouse sensitivity
    public float minYAngle = -20f; // Limit vertical rotation
    public float maxYAngle = 80f;  // Limit vertical rotation
    public Vector3 offset;

    private float _rotationX;
    private float _rotationY;

    void LateUpdate()
    {
        if (target == null) return;

        // Mouse Input for Camera Rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        _rotationX += mouseX;
        _rotationY -= mouseY;

        // Clamp vertical rotation
        _rotationY = Mathf.Clamp(_rotationY, minYAngle, maxYAngle);

        // Calculate rotation
        Quaternion rotation = Quaternion.Euler(_rotationY, _rotationX, 0);

        // Calculate desired position based on rotation
        Vector3 desiredPosition = target.position + offset + rotation * Vector3.back * distance + Vector3.up * height;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Rotate the camera to look at the target
        transform.rotation = rotation;
        transform.LookAt(target.position + offset);
    }
}