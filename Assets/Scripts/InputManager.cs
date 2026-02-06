using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInput _input;

    public event Action<Vector2> OnMove;
    public event Action<bool> OnSprint;
    public event Action<bool> OnUse;

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
            return;
        }

        if (_input == null)
        {
            _input = new PlayerInput();
        }
    }

    private void OnEnable()
    {
        if (Instance != this)
        {
            return;
        }
        if (_input == null) _input = new PlayerInput();

        Unsubscribe();
        Subscribe();

        _input.Player.Enable();
    }

    private void OnDisable()
    {
        if (Instance != this)
        {
            return;
        }
        Unsubscribe();
        _input?.Player.Disable();
    }

    private void Subscribe()
    {
        if (_input == null)
        {
            return;
        }

        _input.Player.Move.performed += OnMovePerformed;
        _input.Player.Move.canceled  += OnMoveCanceled;
        _input.Player.Sprint.performed += OnSprintPerformed;
        _input.Player.Sprint.canceled  += OnSprintCancelled;
        _input.Player.Use.performed += OnUsePerformed;
        _input.Player.Use.canceled += OnUseCancelled;
    }

    private void Unsubscribe()
    {
        if (_input == null)
        {
            return;
        }

        _input.Player.Move.performed -= OnMovePerformed;
        _input.Player.Move.canceled  -= OnMoveCanceled;
        _input.Player.Sprint.performed -= OnSprintPerformed;
        _input.Player.Sprint.canceled  -= OnSprintCancelled;
        _input.Player.Use.performed -= OnUsePerformed;
        _input.Player.Use.canceled -= OnUseCancelled;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        OnMove?.Invoke(context.ReadValue<Vector2>());
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        OnMove?.Invoke(Vector2.zero);
    }

    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        OnSprint?.Invoke(context.ReadValueAsButton());
    }

    private void OnSprintCancelled(InputAction.CallbackContext context)
    {
        OnSprint?.Invoke(false);
    }

    private void OnUsePerformed(InputAction.CallbackContext context)
    {
        OnUse?.Invoke(context.ReadValueAsButton());
    }

    private void OnUseCancelled(InputAction.CallbackContext context)
    {
        OnUse?.Invoke(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
        Unsubscribe();
        _input?.Dispose();
        _input = null;
    }
}
