using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : CharacterBase
{
    PlayerController playerController;
    PlayerStatComponent playerStat { get { return GetStatComponent<PlayerStatComponent>(); } }

    protected override void Awake()
    {
        base.Awake();
        playerController = GetController<PlayerController>();
    }
    protected override void Start()
    {
        base.Start();
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
        animator.SetFloat(AnimParam.MoveSpeed, playerStat.GetStatValue(StatType.MoveSpeed).FinalValue, 0.1f, Time.deltaTime);
    }

    public void EnterSprint()
    {
        playerStat.OnSprintEnter();

    }

    public void ExitSprint()
    {
        playerStat.OnSprintExit();

    }

    public void TryJump()
    {
        if (characterMovement.CanJump())
        {
            characterMovement.Jump();
            playerStat.OnJump();


        }

    }

    public void TryAttack()
    {
        animator.SetTrigger(AnimParam.Attack);

        Vector3 origin = transform.position + transform.forward * 0.4f + transform.up * 2f;

        LayerMask layerMask = LayerMask.GetMask(new string[] { "Enemy", "Resource" });
        var hits = Physics.SphereCastAll(origin, 0.2f, Vector3.down, 2f, layerMask);

        foreach (RaycastHit hit in hits)
        {
            Debug.Log(hit.transform.name);
        }
    }
}
