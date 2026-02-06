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
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }
    
    void Start()
    {
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

    private void Subscribe()
    {
        InputManager.Instance.OnMove += OnMove;
    }

    private void Unsubscribe()
    {
        InputManager.Instance.OnMove -= OnMove;
    }

    private void OnMove(Vector2 value)
    {
        _moveInput = value;
        
        if (value.x < 0f)
        {
            _spriteRenderer.flipX = true;
        }
        else if (value.x >= 1f)
        {
            _spriteRenderer.flipX = false;
        }
    }

    void Update()
    {
        float targetSpeed = _moveInput.x * movementSpeed;
        _currentVelocityX = Mathf.MoveTowards(_currentVelocityX, targetSpeed, acceleration * Time.deltaTime);
        transform.Translate(_currentVelocityX * Time.deltaTime, 0f, 0f);
    }
}