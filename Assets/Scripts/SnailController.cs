using UnityEngine;

public class SnailController : MonoBehaviour
{
    private Vector2 _moveInput = Vector2.zero;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float acceleration = 25f;

    [Header("Sprint Settings")]
    [SerializeField] private float chargeDuration = 1f;
    [SerializeField] private float sprintDuration = 0.5f;
    [SerializeField] private float cooldown = 10f;

    private float _currentVelocityX;

    private bool _isCharging;
    private bool _charged;
    private bool _isSprinting;

    private float _chargeEndTime;
    private float _sprintEndTime;
    private float _cooldownEndTime;

    private float _sprintDirection;
    
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private bool _subscribed;

    void Start()
    {
        if (InputManager.Instance != null)
        {
            Subscribe();
        }
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
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
        if (_subscribed) return;
        if (InputManager.Instance == null) return;
        InputManager.Instance.OnMove += OnMove;
        InputManager.Instance.OnSprint += OnSprint;
        _subscribed = true;
    }

    private void Unsubscribe()
    {
        if (!_subscribed) return;
        if (InputManager.Instance == null) return;
        InputManager.Instance.OnMove -= OnMove;
        InputManager.Instance.OnSprint -= OnSprint;
        _subscribed = false;
    }

    private void OnMove(Vector2 value)
    {
        _animator.SetBool("Move", true);
        _moveInput = value;

        if (_charged && !_isSprinting)
        {
            if (value.x > 0.1f)
                StartSprint(1f);
            else if (value.x < -0.1f)
                StartSprint(-1f);
        }

        if (value.x < 0f)
        {
            _spriteRenderer.flipX = false;
            _animator.Play("Snail-move");
        }
        else if (value.x >= 1f)
        {
            _spriteRenderer.flipX = true;
            _animator.Play("Snail-move");
        }
        else
        {
            _animator.SetBool("Move", false);
        }
    }

    private void OnSprint(bool value)
    {
        if (value && !_isCharging && !_charged && !_isSprinting && Time.time >= _cooldownEndTime)
        {
            _animator.SetBool("Charge", true);
            _isCharging = true;
            _chargeEndTime = Time.time + chargeDuration;
        }
    }

    private void StartSprint(float direction)
    {
        _charged = false;
        _isSprinting = true;
        _sprintDirection = direction;
        _sprintEndTime = Time.time + sprintDuration;
    }

    void Update()
    {
        if (_isCharging && Time.time >= _chargeEndTime)
        {
            _animator.SetBool("Charge", false);
            _isCharging = false;

            if (Mathf.Abs(_moveInput.x) > 0.1f)
            {
                StartSprint(Mathf.Sign(_moveInput.x));
            }
            else
            {
                _charged = true;
            }
        }


        if (_isSprinting && Time.time >= _sprintEndTime)
        {
            _isSprinting = false;
            _cooldownEndTime = Time.time + cooldown;
        }

        float targetSpeed;

        if (_isSprinting)
        {
            targetSpeed = _sprintDirection * sprintSpeed;
        }
        else
        {
            targetSpeed = _moveInput.x * movementSpeed;
        }

        _currentVelocityX = Mathf.MoveTowards(
            _currentVelocityX,
            targetSpeed,
            acceleration * Time.deltaTime
        );

        transform.Translate(_currentVelocityX * Time.deltaTime, 0f, 0f);
    }
}
