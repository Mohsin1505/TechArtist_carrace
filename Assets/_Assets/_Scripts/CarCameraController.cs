using UnityEngine;

public class CarCameraController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform carTransform;
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -10f);

    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private bool smoothFollow = true;

    [Header("Look-Ahead Settings")]
    [SerializeField] private bool enableLookAhead = true;
    [SerializeField] private float lookAheadDistance = 2f;
    [SerializeField] private float lookAheadSpeed = 3f;

    [Header("Tilt Settings")]
    [SerializeField] private bool enableTilt = true;
    [SerializeField] private float maxTiltAngle = 5f;
    [SerializeField] private float tiltSpeed = 5f;

    [Header("Bounds")]
    [SerializeField] private bool clampCameraBounds = false;
    [SerializeField] private Vector2 minMaxXPos = new Vector2(-5f, 5f);
    [SerializeField] private Vector2 minMaxZPos = new Vector2(-10f, 10f);

    private Vector3 currentVelocity;
    private Vector3 lastCarPosition;
    private float currentTilt;

    private void Start()
    {
        if (carTransform == null)
            carTransform = FindObjectOfType<CarController>()?.transform;

        if (carTransform != null)
            lastCarPosition = carTransform.position;

        // Initialize camera position
        if (carTransform != null)
            transform.position = carTransform.position + offset;
    }

    private void LateUpdate()
    {
        if (carTransform == null) return;

        // Calculate target position
        Vector3 targetPosition = carTransform.position + offset;

        // Add look-ahead based on car's movement
        if (enableLookAhead)
        {
            Vector3 carVelocity = (carTransform.position - lastCarPosition) / Time.deltaTime;
            Vector3 lookAheadOffset = carVelocity.normalized * lookAheadDistance * Mathf.Clamp01(carVelocity.magnitude / 20f);
            targetPosition += new Vector3(lookAheadOffset.x, 0, lookAheadOffset.z);
            lastCarPosition = carTransform.position;
        }

        // Apply bounds clamping if enabled
        if (clampCameraBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minMaxXPos.x, minMaxXPos.y);
            targetPosition.z = Mathf.Clamp(targetPosition.z, minMaxZPos.x, minMaxZPos.y);
        }

        // Smooth or direct positioning
        if (smoothFollow)
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        else
            transform.position = targetPosition;

        // Handle camera tilt based on horizontal movement
        if (enableTilt && carTransform != null)
        {
            float horizontalDelta = carTransform.position.x - transform.position.x;
            float targetTilt = Mathf.Clamp(horizontalDelta * 0.5f, -maxTiltAngle, maxTiltAngle);
            currentTilt = Mathf.Lerp(currentTilt, targetTilt, tiltSpeed * Time.deltaTime);

            // Apply rotation (keep looking at car)
            Vector3 lookAtPosition = carTransform.position;
            lookAtPosition.y = transform.position.y;
            transform.LookAt(lookAtPosition);
            transform.Rotate(0, 0, currentTilt);
        }
        else
        {
            // Simple follow - always look at car
            Vector3 lookAtPosition = carTransform.position;
            lookAtPosition.y = transform.position.y;
            transform.LookAt(lookAtPosition);
        }
    }

    // Public method to change camera target dynamically
    public void SetCarTarget(Transform newCarTransform)
    {
        carTransform = newCarTransform;
        if (carTransform != null)
            lastCarPosition = carTransform.position;
    }

    // Public method to reset camera position instantly
    public void SnapToCar()
    {
        if (carTransform != null)
            transform.position = carTransform.position + offset;
    }

    // Visualize camera bounds in editor
    private void OnDrawGizmosSelected()
    {
        if (clampCameraBounds)
        {
            Gizmos.color = Color.green;
            Vector3 center = new Vector3((minMaxXPos.x + minMaxXPos.y) / 2, 0, (minMaxZPos.x + minMaxZPos.y) / 2);
            Vector3 size = new Vector3(minMaxXPos.y - minMaxXPos.x, 10f, minMaxZPos.y - minMaxZPos.x);
            Gizmos.DrawWireCube(center, size);
        }
    }
}