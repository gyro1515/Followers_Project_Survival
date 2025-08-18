using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : CharacterBase
{
    PlayerController playerController;

    
    protected override void Awake()
    {
        base.Awake();

        playerController = controller as PlayerController;

    }

    protected override void Update()
    {
        base.Update();

        


    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

    }

    private void LateUpdate()
    {
        animator.SetBool(AnimParam.IsMoving, playerController.MoveInput != Vector2.zero);
        animator.SetFloat(AnimParam.Forward, playerController.MoveInput.y);
        animator.SetFloat(AnimParam.Right, playerController.MoveInput.x);
        animator.SetBool(AnimParam.IsJumping, characterMovement.IsJumping);
        animator.SetBool(AnimParam.IsFalling, characterMovement.IsFalling);
        animator.SetBool(AnimParam.IsGrounded, characterMovement.IsGrounded);
    }

    public override ControllerBase GetController()
    {
        return playerController;
    }

    public void TryJump()
    {
        characterMovement.Jump();
    }
}
