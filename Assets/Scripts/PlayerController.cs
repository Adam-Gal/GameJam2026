using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 _moveInput = Vector2.zero;
    [SerializeField]
    private float movementSpeed;

    private float _movementX;
    private float _movementY;

    void OnEnable()
    {
        Subscribe();
    }

    void OnDisable()
    {
        Unsubscribe();
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
        _moveInput = value.normalized;
    }

    void Update()
    {
        _movementX = Mathf.Lerp(_movementX, _moveInput.x, movementSpeed * Time.deltaTime);
        _movementY = Mathf.Lerp(_movementY, _moveInput.y, movementSpeed * Time.deltaTime);
        transform.Translate(_movementX * movementSpeed * Time.deltaTime, _movementY * movementSpeed * Time.deltaTime, 0);
    }
}