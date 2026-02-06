using UnityEngine;

public class FishController : MonoBehaviour
{
    private Vector2 _moveInput = Vector2.zero;

    [Header("Movement")]
    [SerializeField]
    private float movementSpeed = 5f;
    [SerializeField]
    private float acceleration = 25f;

    private float _currentVelocityX;
    
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
        _subscribed = true;
    }

    private void Unsubscribe()
    {
        if (!_subscribed) return;
        if (InputManager.Instance == null) return;
        InputManager.Instance.OnMove -= OnMove;
        _subscribed = false;
    }

    private void OnMove(Vector2 value)
    {
        _moveInput = value;
        
        if (value.x < 0f)
        {
            _spriteRenderer.flipX = false;
        }
        else if (value.x >= 1f)
        {
            _spriteRenderer.flipX = true;
        }
    }

    void Update()
    {
        float targetSpeed = _moveInput.x * movementSpeed;
        _currentVelocityX = Mathf.MoveTowards(_currentVelocityX, targetSpeed, acceleration * Time.deltaTime);
        transform.Translate(_currentVelocityX * Time.deltaTime, 0f, 0f);
    }
}