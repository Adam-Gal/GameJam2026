using UnityEngine;

public class SnailController : MonoBehaviour
{
    private Vector2 _moveInput = Vector2.zero;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float acceleration = 25f;

    [Header("Sprint Settings")]
    [SerializeField] private float sprintDuration = 0.5f;
    [SerializeField] private float cooldown = 10f;

    private float _currentVelocityX;
    private bool _isSprinting;
    private float _sprintEndTime;
    private float _cooldownEndTime;

    void Start()
    {
        if (InputManager.Instance != null)
            Subscribe();
    }

    void OnEnable()
    {
        if (InputManager.Instance != null)
            Subscribe();
    }

    void OnDisable()
    {
        if (InputManager.Instance != null)
            Unsubscribe();
    }

    private void Subscribe()
    {
        InputManager.Instance.OnMove += OnMove;
        InputManager.Instance.OnSprint += OnSprint;
    }

    private void Unsubscribe()
    {
        InputManager.Instance.OnMove -= OnMove;
        InputManager.Instance.OnSprint -= OnSprint;
    }

    private void OnMove(Vector2 value)
    {
        _moveInput = value;
    }

    private void OnSprint(bool value)
    {
        if (value && Time.time >= _cooldownEndTime)
        {
            _isSprinting = true;
            _sprintEndTime = Time.time + sprintDuration;
            _cooldownEndTime = Time.time + cooldown;
        }
    }

    void Update()
    {
        if (_isSprinting && Time.time >= _sprintEndTime)
        {
            _isSprinting = false;
        }

        float targetSpeed = _moveInput.x * ( _isSprinting ? sprintSpeed : movementSpeed );
        _currentVelocityX = Mathf.MoveTowards(_currentVelocityX, targetSpeed, acceleration * Time.deltaTime);
        transform.Translate(_currentVelocityX * Time.deltaTime, 0f, 0f);
    }
}
