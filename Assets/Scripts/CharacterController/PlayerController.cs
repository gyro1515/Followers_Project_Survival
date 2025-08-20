using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Constants;
using Cinemachine;
using Unity.VisualScripting;

// 입력 처리만
public class PlayerController : ControllerBase
{
    PlayerCharacter player;
    PlayerInputActions playerInputActions;
    InteractionComponet interactionComponet;
    [Header("Camera")]
    Vector2 lookInput;
    public Transform cameraTarget;
    public float HorizontalSensitivity = 1f;
    public float VerticalSensitivity = 0.01f;
    public float MaxCameraPitch = 40f;
    public float MinCameraPitch = -40f;
    float cameraPitch = 0f;
    public CinemachineVirtualCamera virtualCamera;

    // 추후 다른 곳으로 옮겨도 됩니다
    bool canControl = true; // 인벤토리나 대화 창 열릴 시 Look, Move 등 입력 안들어가도록

    protected override void Awake()
    {
        base.Awake();

        if (player == null)
        {
            player = GetComponent<PlayerCharacter>();
        }

        playerInputActions = new PlayerInputActions();

        if (virtualCamera == null)
        {
            virtualCamera = Camera.main.transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
        }
        if (interactionComponet == null)
        {
            interactionComponet = GetComponent<InteractionComponet>();
        }

        virtualCamera.Follow = cameraTarget;

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
        playerInputActions.Player.Attack.performed += OnAttack;
        playerInputActions.Player.Interaction.started += OnInteraction;

    }

    private void OnDisable()
    {
        playerInputActions.Player.Move.performed -= OnMove;
        playerInputActions.Player.Move.canceled -= OnMove;
        playerInputActions.Player.Look.performed -= OnLook;
        playerInputActions.Player.Look.canceled -= OnLook;
        playerInputActions.Player.Jump.performed -= OnJump;
        playerInputActions.Player.Attack.performed -= OnAttack;
        playerInputActions.Player.Interaction.started -= OnInteraction;

        playerInputActions.Player.Disable();
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (!canControl) return;
        player.TryAttack();
    }
    private void OnJump(InputAction.CallbackContext context)
    {
        if (!canControl) return;
        player.TryJump();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        if (!canControl) return; 
        lookInput = context.ReadValue<Vector2>();

        transform.Rotate(Vector3.up * lookInput.x * HorizontalSensitivity);

        cameraPitch -= lookInput.y * VerticalSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, MinCameraPitch, MaxCameraPitch);

        cameraTarget.localEulerAngles = new Vector3(cameraPitch, 0, 0);

    }
    private void OnInteraction(InputAction.CallbackContext context)
    {
        if (!canControl) return;
        interactionComponet?.OnIteract();
    }
    public void SetControlActive(bool active)
    {
        canControl = active;
    }
    
}
