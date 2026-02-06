using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Audio")]
    [SerializeField] private AudioSource moveAudio;
    [SerializeField] private AudioSource chargeAudio;

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

        StopMoveSound();
        StopChargeSound();
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
        _moveInput = value;
        _animator.SetBool("Move", Mathf.Abs(value.x) > 0.1f);

        if (value.x < 0f)
        {
            _spriteRenderer.flipX = false;
            _animator.Play("Snail-move");
        }
        else if (value.x > 0f)
        {
            _spriteRenderer.flipX = true;
            _animator.Play("Snail-move");
        }

        if (_charged && !_isSprinting)
        {
            if (value.x > 0.1f)
                StartSprint(1f);
            else if (value.x < -0.1f)
                StartSprint(-1f);
        }
    }

    private void OnSprint(bool value)
    {
        if (!value) return;

        if (_isCharging || _charged || _isSprinting) return;
        if (Time.time < _cooldownEndTime) return;

        _animator.SetBool("Charge", true);
        _isCharging = true;
        _chargeEndTime = Time.time + chargeDuration;

        PlayChargeSound();
        StopMoveSound();
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
            _isCharging = false;
            StopChargeSound();

            if (Mathf.Abs(_moveInput.x) > 0.1f)
                StartSprint(Mathf.Sign(_moveInput.x));
            else
                _charged = true;
        }
        
        if (_isSprinting && Time.time >= _sprintEndTime)
        {
            _isSprinting = false;
            _cooldownEndTime = Time.time + cooldown;
            _animator.SetBool("Charge", false);
        }
        
        float targetSpeed = _isSprinting
            ? _sprintDirection * sprintSpeed
            : _moveInput.x * movementSpeed;

        _currentVelocityX = Mathf.MoveTowards(
            _currentVelocityX,
            targetSpeed,
            acceleration * Time.deltaTime
        );

        transform.Translate(_currentVelocityX * Time.deltaTime, 0f, 0f);
        
        bool isMoving = Mathf.Abs(_currentVelocityX) > 0.1f;

        if (_isCharging)
        {
            StopMoveSound();
        }
        else if (isMoving)
        {
            PlayMoveSound();
        }
        else
        {
            StopMoveSound();
        }
    }
    
    private void PlayMoveSound()
    {
        if (moveAudio != null && !moveAudio.isPlaying)
            moveAudio.Play();
    }

    private void StopMoveSound()
    {
        if (moveAudio != null && moveAudio.isPlaying)
            moveAudio.Stop();
    }

    private void PlayChargeSound()
    {
        if (chargeAudio != null && !chargeAudio.isPlaying)
            chargeAudio.Play();
    }

    private void StopChargeSound()
    {
        if (chargeAudio != null && chargeAudio.isPlaying)
            chargeAudio.Stop();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("End1"))
        {
            SceneManager.LoadScene("List 2");
        }
        if (other.gameObject.CompareTag("End2"))
        {
            SceneManager.LoadScene("List 3");
        }
        if (other.gameObject.CompareTag("End3"))
        {
            SceneManager.LoadScene("List 4");
        }
    }
}
