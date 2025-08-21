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
    }

    public void TryJump()
    {
        characterMovement.Jump();
    }

    public void TryAttack()
    {
        animator.SetTrigger(AnimParam.Attack);
        LayerMask layerMask = LayerMask.GetMask(new string[] { "Enemy", "Resource" });

        Vector3 origin = transform.position + transform.forward * 0.4f + transform.up * 2f;
        var hits = Physics.SphereCastAll(origin, 0.2f, Vector3.down, 2f, layerMask);


        foreach (RaycastHit hit in hits)
        {
            Debug.DrawLine(origin, hit.point, Color.red, 1.0f);
            Debug.DrawRay(hit.point, hit.normal, Color.magenta, 1.0f);
            if (hit.collider.TryGetComponent(out Resource resource)) // 캘 수 있는 자원이라면 캐기
            {
                resource.Gather(gameObject.transform, 1);
            }
            // 적이라면 적에게 데미지
        }
    }
    
}
