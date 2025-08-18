using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Constants;

// 입력 처리만
public class PlayerController : ControllerBase
{
    PlayerCharacter player;
    PlayerInput playerInput;


    protected override void Awake()
    {
        base.Awake();

        if (player == null)
        {
            player = GetComponent<PlayerCharacter>();
        }

        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }

        

    }

    protected override void Update()
    {
        base.Update();

    }

    private void HandleInputActionTriggered(InputAction.CallbackContext context)
    {
        switch (context.action.name)
        {
            case PlayerInputAction.Move:
                OnMove(context);
                break;
            case PlayerInputAction.Look:
                OnLook(context);
                break;
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext context)
    {

    }

    private void OnEnable()
    {
        playerInput.onActionTriggered += HandleInputActionTriggered;
    }

    private void OnDisable()
    {
        playerInput.onActionTriggered -= HandleInputActionTriggered;
    }
}
