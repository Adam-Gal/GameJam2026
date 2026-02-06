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
    public event Action<bool> OnOne;
    public event Action<bool> OnTwo;
    public event Action<bool> OnThree;

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
        _input.Player.One.performed += OnOnePerformed;
        _input.Player.One.canceled += OnOneCancelled;
        _input.Player.Two.performed += OnTwoPerformed;
        _input.Player.Two.canceled += OnTwoCancelled;
        _input.Player.Three.performed += OnThreePerformed;
        _input.Player.Three.canceled += OnThreeCancelled;
        _input.Player.Escape.performed += OnEscapePerformed;
        _input.Player.Escape.canceled += OnEscapeCancelled;
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
        _input.Player.One.performed -= OnOnePerformed;
        _input.Player.One.canceled -= OnOneCancelled;
        _input.Player.Two.performed -= OnTwoPerformed;
        _input.Player.Two.canceled -= OnTwoCancelled;
        _input.Player.Three.performed -= OnThreePerformed;
        _input.Player.Three.canceled -= OnThreeCancelled;
        _input.Player.Escape.performed -= OnEscapePerformed;
        _input.Player.Escape.canceled -= OnEscapeCancelled;
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
    
    private void OnOnePerformed(InputAction.CallbackContext context)
    {
        OnOne?.Invoke(context.ReadValueAsButton());
    }

    private void OnOneCancelled(InputAction.CallbackContext context)
    {
        OnOne?.Invoke(false);
    }
    
    private void OnTwoPerformed(InputAction.CallbackContext context)
    {
        OnTwo?.Invoke(context.ReadValueAsButton());
    }

    private void OnTwoCancelled(InputAction.CallbackContext context)
    {
        OnTwo?.Invoke(false);
    }
    
    private void OnThreePerformed(InputAction.CallbackContext context)
    {
        OnThree?.Invoke(context.ReadValueAsButton());
    }

    private void OnThreeCancelled(InputAction.CallbackContext context)
    {
        OnThree?.Invoke(false);
    }
    
    private void OnEscapePerformed(InputAction.CallbackContext context)
    {
        Application.Quit();
    }

    private void OnEscapeCancelled(InputAction.CallbackContext context)
    {
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
