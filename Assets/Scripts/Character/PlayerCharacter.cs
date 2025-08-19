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

        playerController = GetController<PlayerController>();
        GameManager.Instance.AddPlayer(this);
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
        animator.SetFloat(AnimParam.Forward, playerController.MoveInput.y, 0.1f, Time.deltaTime);
        animator.SetFloat(AnimParam.Right, playerController.MoveInput.x, 0.1f, Time.deltaTime);
        animator.SetBool(AnimParam.IsJumping, characterMovement.IsJumping);
        animator.SetBool(AnimParam.IsFalling, characterMovement.IsFalling);
        animator.SetBool(AnimParam.IsGrounded, characterMovement.IsGrounded);
    }

    public void TryJump()
    {
        characterMovement.Jump();
    }
}
