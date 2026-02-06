using System.Collections;
using UnityEngine;

public class FishController : MonoBehaviour
{
    private Vector2 _moveInput = Vector2.zero;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float acceleration = 25f;

    [Header("Audio")]
    [SerializeField] private float soundPauseTime = 5f;

    private float _currentVelocityX;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private bool _subscribed;

    private AudioSource _fishAudio;
    private Coroutine _soundCoroutine;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _fishAudio = GetComponent<AudioSource>();

        if (InputManager.Instance != null)
        {
            Subscribe();
        }

        StartFishSound();
    }

    void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            Subscribe();
        }

        StartFishSound();
    }

    void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            Unsubscribe();
        }

        StopFishSound();
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
            _spriteRenderer.flipX = false;
        else if (value.x > 0f)
            _spriteRenderer.flipX = true;
    }

    void Update()
    {
        float targetSpeed = _moveInput.x * movementSpeed;
        _currentVelocityX = Mathf.MoveTowards(
            _currentVelocityX,
            targetSpeed,
            acceleration * Time.deltaTime
        );

        transform.Translate(_currentVelocityX * Time.deltaTime, 0f, 0f);
    }

    private void StartFishSound()
    {
        if (_fishAudio == null || _soundCoroutine != null)
            return;

        _soundCoroutine = StartCoroutine(FishSoundLoop());
    }

    private void StopFishSound()
    {
        if (_soundCoroutine != null)
        {
            StopCoroutine(_soundCoroutine);
            _soundCoroutine = null;
        }

        if (_fishAudio != null && _fishAudio.isPlaying)
        {
            _fishAudio.Stop();
        }
    }

    private IEnumerator FishSoundLoop()
    {
        while (true)
        {
            _fishAudio.pitch = Random.Range(0.95f, 1.05f);
            _fishAudio.Play();
            yield return new WaitForSeconds(_fishAudio.clip.length);
            yield return new WaitForSeconds(soundPauseTime);
        }
    }
}
