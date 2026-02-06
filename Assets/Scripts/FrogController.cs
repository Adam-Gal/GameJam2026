using System.Collections;
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

    [Header("Frog Audio")]
    [SerializeField] private float soundPauseTime = 5f;

    private AudioSource _frogAudio;
    private Coroutine _frogSoundCoroutine;
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
        _frogAudio = GetComponent<AudioSource>();

        SubscribeInput();
        StartFrogSound();
    }

    void OnEnable()
    {
        SubscribeInput();
    }

    void OnDisable()
    {
        UnsubscribeInput();
        
        _using = false;
        _targetRotationZ = 0f;
        if (_rigidbody2D != null)
        {
            _rigidbody2D.gravityScale = 1f;
        }

        if (mainCamera != null)
        {
            mainCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        if (_frogSoundCoroutine != null)
        {
            StopCoroutine(_frogSoundCoroutine);
            _frogSoundCoroutine = null;
        }

        if (_frogAudio != null && _frogAudio.isPlaying)
        {
            _frogAudio.Stop();
        }
    }

    void OnDestroy()
    {
        UnsubscribeInput();
    }

    private void SubscribeInput()
    {
        if (InputManager.Instance == null)
        {
            return;
        }

        InputManager.Instance.OnMove += HandleOnMove;
        InputManager.Instance.OnUse += HandleOnUse;
    }

    private void UnsubscribeInput()
    {
        if (InputManager.Instance == null)
        {
            return;
        }

        InputManager.Instance.OnMove -= HandleOnMove;
        InputManager.Instance.OnUse -= HandleOnUse;
    }

    private void HandleOnMove(Vector2 value)
    {
        if (this == null)
        {
            UnsubscribeInput();
            return;
        }

        if (!IsCurrentCharacter())
        {
            return;
        }

        OnMove(value);
    }

    private void HandleOnUse(bool value)
    {
        if (this == null)
        {
            UnsubscribeInput();
            return;
        }

        if (!IsCurrentCharacter())
        {
            return;
        }

        OnUse(value);
    }

    private bool IsCurrentCharacter()
    {
        if (CharacterManagerController.Instance == null)
        {
            return false;
        }
        GameObject current = CharacterManagerController.Instance.CurrentCharacterGameObject;
        if (current == null)
        {
            return false;
        }
        return current == gameObject;
    }

    private void OnMove(Vector2 value)
    {
        _animator.SetBool("Move", Mathf.Abs(value.x) > DEADZONE);
        _moveInput = value;

        if (value.x < 0f)
        {
            _spriteRenderer.flipX = false;
        }
        else if (value.x > 0f)
        {
            _spriteRenderer.flipX = true;
        }
    }

    private void OnUse(bool value)
    {
        if (!value || Time.time < _nextFlipTime)
        {
            return;
        }

        _using = !_using;
        transform.rotation = Quaternion.Euler(0, 0, _using ? 180f : 0f);
        _targetRotationZ = _using ? 180f : 0f;
        if (_rigidbody2D != null)
        {
            _rigidbody2D.gravityScale = _using ? -1 : 1;
        }

        _nextFlipTime = Time.time + flipCooldown;
    }

    private void Movement()
    {
        if (!_onGround)
        {
            return;
        }

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
        if (mainCamera == null)
        {
            return;
        }

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

    private void StartFrogSound()
    {
        if (_frogAudio == null || _frogSoundCoroutine != null)
        {
            return;
        }
        _frogSoundCoroutine = StartCoroutine(FrogSoundLoop());
    }

    private IEnumerator FrogSoundLoop()
    {
        while (true)
        {
            _frogAudio.pitch = Random.Range(0.95f, 1.05f);
            _frogAudio.Play();
            yield return new WaitForSeconds(_frogAudio.clip.length + soundPauseTime);
        }
    }
}
