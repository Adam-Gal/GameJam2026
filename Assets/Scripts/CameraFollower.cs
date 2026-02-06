using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Transform target;

    [Header("Camera X bounds (world units)")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    private Camera _camera;
    private float _yOffset;
    private float _zOffset;

    void Start()
    {
        _camera = GetComponent<Camera>();

        // Keep only Y and Z offset
        _yOffset = transform.position.y - target.position.y;
        _zOffset = transform.position.z;
    }

    void LateUpdate()
    {
        float halfCameraWidth = _camera.orthographicSize * _camera.aspect;

        float minCameraX = minX + halfCameraWidth;
        float maxCameraX = maxX - halfCameraWidth;

        float targetX = Mathf.Clamp(
            target.position.x,
            minCameraX,
            maxCameraX
        );

        transform.position = new Vector3(
            targetX,
            target.position.y + _yOffset,
            _zOffset
        );
    }
}
