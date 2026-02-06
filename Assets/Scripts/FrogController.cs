using System;
using UnityEngine;

public class FrogController : MonoBehaviour
{
    private Vector2 _moveInput = Vector2.zero;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float rotationSpeed = 5f;

    private float _currentVelocityX;
    public Camera mainCamera;
    private bool _using;
    private Rigidbody2D _rigidbody2D;
    private BoxCollider2D _boxCollider2D;
    private bool _onGround;

    private float _targetRotationZ;
    [Header("Flip Cooldown")]
    [SerializeField] private float flipCooldown = 2f;
    private float _nextFlipTime = 0f;

    void Start()
    {
        if (InputManager.Instance != null) Subscribe();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        if (InputManager.Instance != null) Subscribe();
    }

    void OnDisable()
    {
        if (InputManager.Instance != null) Unsubscribe();
    }

    private void Subscribe()
    {
        InputManager.Instance.OnMove += OnMove;
        InputManager.Instance.OnUse += OnUse;
    }

    private void Unsubscribe()
    {
        InputManager.Instance.OnMove -= OnMove;
        InputManager.Instance.OnUse -= OnUse;
    }

    private void OnMove(Vector2 value)
    {
        _moveInput = value;
    }

    private void OnUse(bool value)
    {
        if (value && Time.time >= _nextFlipTime)
        {
            _using = !_using;
            
            transform.rotation = Quaternion.Euler(0, 0, _using ? 180f : 0f);
            _targetRotationZ = _using ? 180f : 0f;
            _rigidbody2D.gravityScale = _using ? -1 : 1;
            
            _nextFlipTime = Time.time + flipCooldown;
        }
    }

    private void Movement()
    {
        if (!_onGround)
        {
            return;
        }
        
        float targetSpeed = _moveInput.x * movementSpeed;
        _currentVelocityX = Mathf.MoveTowards(_currentVelocityX, targetSpeed, acceleration * Time.deltaTime);
        transform.Translate(_currentVelocityX * Time.deltaTime, 0f, 0f);
    }

    private void Ability()
    {
        float currentRotationZ = mainCamera.transform.rotation.eulerAngles.z;
        float newRotationZ = Mathf.LerpAngle(currentRotationZ, _targetRotationZ, rotationSpeed * Time.deltaTime);
        mainCamera.transform.rotation = Quaternion.Euler(0, 0, newRotationZ);
    }

    void Update()
    {
        Ability();
        Movement();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        _onGround = true;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        _onGround = false;
    }
}
