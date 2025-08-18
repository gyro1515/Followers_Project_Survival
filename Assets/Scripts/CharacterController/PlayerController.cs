using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Constants;

// 입력 처리만
public class PlayerController : ControllerBase
{
    PlayerCharacter player;
    PlayerInputActions playerInputActions;

    [Header("Camera")]
    Vector2 lookInput;
    public Transform cameraTarget;
    public float HorizontalSensitivity = 1f;
    public float VerticalSensitivity = 0.01f;
    public float MaxCameraPitch = 40f;
    public float MinCameraPitch = -40f;
    float cameraPitch = 0f;


    protected override void Awake()
    {
        base.Awake();

        if (player == null)
        {
            player = GetComponent<PlayerCharacter>();
        }

        playerInputActions = new PlayerInputActions();

    }

    protected override void Update()
    {
        base.Update();

    }

    private void OnEnable()
    {
        playerInputActions.Player.Enable();

        playerInputActions.Player.Move.performed += OnMove;
        playerInputActions.Player.Move.canceled += OnMove;
        playerInputActions.Player.Look.performed += OnLook;
        playerInputActions.Player.Look.canceled += OnLook;
        playerInputActions.Player.Jump.performed += OnJump;

    }

    private void OnDisable()
    {
        playerInputActions.Player.Move.performed -= OnMove;
        playerInputActions.Player.Move.canceled -= OnMove;
        playerInputActions.Player.Look.performed -= OnLook;
        playerInputActions.Player.Look.canceled -= OnLook;
        playerInputActions.Player.Jump.performed -= OnJump;

        playerInputActions.Player.Disable();
    }
    private void OnJump(InputAction.CallbackContext context)
    {
        player.TryJump();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();

        transform.Rotate(Vector3.up * lookInput.x * HorizontalSensitivity);

        cameraPitch -= lookInput.y * VerticalSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, MinCameraPitch, MaxCameraPitch);

        cameraTarget.localEulerAngles = new Vector3(cameraPitch, 0, 0);

    }
}
