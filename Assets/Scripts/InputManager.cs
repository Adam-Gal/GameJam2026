using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInput _input;
    public event Action<Vector2> OnMove;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
        _input = new PlayerInput();
    }

    private void EnableActions()
    {
        _input.Player.Move.Enable();
    }

    private void DisableActions()
    {
        _input.Player.Move.Disable();

    }

    private void Subscribe()
    {
        _input.Player.Move.performed += OnMovePerformed;
        _input.Player.Move.canceled  += OnMoveCanceled;
    }

    private void UnSubscribe()
    {
        _input.Player.Move.performed -= OnMovePerformed;
        _input.Player.Move.canceled  -= OnMoveCanceled;
    }

    private void OnEnable()
    {
        if (_input == null)
        {
            _input = new PlayerInput();
        }
        
        EnableActions();
        Subscribe();
    }

    private void OnDisable()
    {
        UnSubscribe();
        DisableActions();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        OnMove?.Invoke(value);
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        OnMove?.Invoke(Vector2.zero);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}