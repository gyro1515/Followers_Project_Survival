using Cinemachine;
using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

// 입력 처리만
public class PlayerController : ControllerBase
{
    PlayerCharacter player;
    PlayerInputActions playerInputActions;
    InteractionComponet interactionComponet;
    public event Action OnInventotyAction;
    public event Action OnBuildAction;
    [Header("Camera")]
    Vector2 lookInput;
    public Transform cameraTarget;
    public float HorizontalSensitivity = 1f;
    public float VerticalSensitivity = 0.01f;
    public float MaxCameraPitch = 40f;
    public float MinCameraPitch = -40f;
    float cameraPitch = 0f;
    public CinemachineVirtualCamera virtualCamera;

    

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

        if (Input.GetKeyDown(KeyCode.V))
        {
            player.GetStatComponent<PlayerStatComponent>().statValues[StatType.Health].BaseValue -= 10f;
            player.GetStatComponent<PlayerStatComponent>().statValues[StatType.Stamina].BaseValue -= 10f;
            player.GetStatComponent<PlayerStatComponent>().statValues[StatType.Hunger].BaseValue -= 10f;
            player.GetStatComponent<PlayerStatComponent>().statValues[StatType.Thirst].BaseValue -= 10f;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            player.GetStatComponent<PlayerStatComponent>().GetStatValue(StatType.MoveSpeed).BaseValue -= 0.5f;
        }
        
    }

    private void OnEnable()
    {
        playerInputActions.Player.Enable();
        playerInputActions.PlayerUI.Enable();

        playerInputActions.Player.Move.performed += OnMove;
        playerInputActions.Player.Move.canceled += OnMove;
        playerInputActions.Player.Look.performed += OnLook;
        playerInputActions.Player.Look.canceled += OnLook;
        playerInputActions.Player.Jump.performed += OnJump;
        playerInputActions.Player.Attack.performed += OnAttack;
        playerInputActions.Player.Interaction.started += OnInteraction;
        playerInputActions.Player.Sprint.started += Sprint_started;
        playerInputActions.Player.Sprint.canceled += Sprint_canceled;
        playerInputActions.Player.Repair.started += OnRepair;

        playerInputActions.PlayerUI.Inventory.started += OnInventory;
        playerInputActions.PlayerUI.Build.started += OnBuild;

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
        playerInputActions.Player.Sprint.started -= Sprint_started;
        playerInputActions.Player.Sprint.canceled -= Sprint_canceled;
        playerInputActions.Player.Repair.started -= OnRepair;
        playerInputActions.PlayerUI.Inventory.started -= OnInventory;
        playerInputActions.PlayerUI.Build.started -= OnBuild;

        playerInputActions.Player.Disable();
        playerInputActions.PlayerUI.Disable();
    }

    private void Sprint_canceled(InputAction.CallbackContext context)
    {
        player.ExitSprint();
    }

    private void Sprint_started(InputAction.CallbackContext context)
    {
        player.EnterSprint();
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        player.TryAttack();
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
    private void OnInteraction(InputAction.CallbackContext context)
    {
        interactionComponet?.OnIteract();
    }
    private void OnInventory(InputAction.CallbackContext context)
    {
        OnInventotyAction?.Invoke();
    }
    private void OnBuild(InputAction.CallbackContext context)
    {
        OnBuildAction?.Invoke();
    }
    private void OnRepair(InputAction.CallbackContext context)
    {
        Debug.Log("플레이어 컨트롤러에서 실행");
        interactionComponet?.OnRepair();
    }
    public void SetControlActive(bool active)
    {
        // 인벤토리는 제외해야??
        if (active) playerInputActions.Player.Enable();
        else playerInputActions.Player.Disable();
    }
    /*public void SetControlActiveTogle()
    {
        if (playerInputActions.Player.enabled)
        {
            playerInputActions.Player.Disable();
        }
        else playerInputActions.Player.Enable();
    }*/
    
}
