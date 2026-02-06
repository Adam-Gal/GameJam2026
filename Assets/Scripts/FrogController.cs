using System;
using UnityEngine;

public class FrogController : MonoBehaviour
{
    private Vector2 _moveInput = Vector2.zero;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float rotationSpeed = 5f;

    private float _currentVelocityX;
    public Camera mainCamera;
    private bool _using;
    private Rigidbody2D _rigidbody2D;
    private bool _onGround;

    private float _targetRotationZ;

    [Header("Flip Cooldown")]
    [SerializeField] private float flipCooldown = 2f;
    private float _nextFlipTime;

    [Header("Side Movement Timing")]
    [SerializeField] private float moveDuration = 0.25f;
    [SerializeField] private float waitDuration = 0.65f;

    private float _sideTimer;
    private float _nextHopAllowedTime;
    private bool _isSideMoving;
    private bool _hadSideInput;
    
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private const float DEADZONE = 0.1f;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        if (InputManager.Instance != null)
        {
            Subscribe();
        }
    }

    void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            Subscribe();
        }
    }

    void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            Unsubscribe();
        }
    }

    void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            Unsubscribe();
        }
    }

    private void Subscribe()
    {
        InputManager.Instance.OnMove += HandleOnMove;
        InputManager.Instance.OnUse += HandleOnUse;
    }

    private void Unsubscribe()
    {
        InputManager.Instance.OnMove -= HandleOnMove;
        InputManager.Instance.OnUse -= HandleOnUse;
    }

    private void HandleOnMove(Vector2 value)
    {
        if (this == null) return;
        OnMove(value);
    }

    private void HandleOnUse(bool value)
    {
        if (this == null) return;
        OnUse(value);
    }

    private void OnMove(Vector2 value)
    {
        _animator.SetBool("Move", true);
        _moveInput = value;
        
        if (value.x < 0f)
        {
            _spriteRenderer.flipX = false;
        }
        else if (value.x >= 1f)
        {
            _spriteRenderer.flipX = true;
        }
        else
        {
            _animator.SetBool("Move", false);
        }
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
            return;

        bool hasSideInput = Mathf.Abs(_moveInput.x) > DEADZONE;

        if (!hasSideInput)
        {
            _currentVelocityX = 0f;
            _sideTimer = 0f;
            _isSideMoving = false;
            _hadSideInput = false;
            return;
        }

        if (!_hadSideInput && Time.time >= _nextHopAllowedTime)
        {
            _isSideMoving = true;
            _sideTimer = 0f;
            _hadSideInput = true;
            _nextHopAllowedTime = Time.time + moveDuration + waitDuration;
        }

        if (!_hadSideInput)
            return;

        _sideTimer += Time.deltaTime;

        if (_isSideMoving)
        {
            if (_sideTimer >= moveDuration)
            {
                _sideTimer = 0f;
                _isSideMoving = false;
                _currentVelocityX = 0f;
            }
            else
            {
                _currentVelocityX = Mathf.Sign(_moveInput.x) * movementSpeed;
                transform.Translate(_currentVelocityX * Time.deltaTime, 0f, 0f);
            }
        }
        else
        {
            if (_sideTimer >= waitDuration)
            {
                _sideTimer = 0f;
                _isSideMoving = true;
            }
        }
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
