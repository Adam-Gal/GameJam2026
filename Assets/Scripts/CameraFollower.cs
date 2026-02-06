using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollower : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera X bounds (world units)")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    [Header("Follow settings")]
    [SerializeField] private float smoothSpeed = 8f;

    private Camera _camera;
    private Vector3 _offset;

    void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    void Start()
    {
        if (target)
            RecalculateOffset();
    }

    void LateUpdate()
    {
        if (!target) return;

        float halfCameraWidth = _camera.orthographicSize * _camera.aspect;

        float minCameraX = minX + halfCameraWidth;
        float maxCameraX = maxX - halfCameraWidth;

        float clampedX = Mathf.Clamp(
            target.position.x,
            minCameraX,
            maxCameraX
        );

        Vector3 desiredPosition = new Vector3(
            clampedX,
            target.position.y + _offset.y,
            _offset.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
    }

    // === VOLAJ PRI SWITCHI POSTÁV ===
    public void SetTarget(Transform newTarget, bool instantSnap = false)
    {
        if (newTarget == null) return;

        target = newTarget;
        RecalculateOffset();

        if (instantSnap)
            transform.position = target.position + _offset;
    }

    private void RecalculateOffset()
    {
        _offset = transform.position - target.position;
        _offset.x = 0f; // X rieši clamp
    }
}