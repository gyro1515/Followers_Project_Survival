using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : CharacterBase
{
    PlayerController playerController;
    PlayerStatComponent playerStat { get { return GetStatComponent<PlayerStatComponent>(); } }
    EquipmentController equipmentController;
    AnimatorStateInfo currentAttackStateInfo;
    bool isAttacking;
    [SerializeField] Transform dropTransform;
    protected override void Awake()
    {
        base.Awake();
        playerController = GetController<PlayerController>();
        equipmentController = GetComponent<EquipmentController>();
        equipmentController.OnWeaponEquipped += SetEquipped;
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

        AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(1);
        
        if (animatorStateInfo.IsName("MeleeAttack1") || animatorStateInfo.IsName("MeleeAttack2") || animatorStateInfo.IsName("PunchLeft") || animatorStateInfo.IsName("PunchRight"))
        {
            if (currentAttackStateInfo.fullPathHash != animatorStateInfo.fullPathHash)
            {
                isAttacking = false;
            }
            currentAttackStateInfo = animatorStateInfo;
        }
        
        if (animatorStateInfo.IsName("MeleeAttack1") || animatorStateInfo.IsName("MeleeAttack2"))
        {
            if (!isAttacking && animatorStateInfo.normalizedTime > 0.46f)
            {
                isAttacking = true;
                Attack();

            }
            
        }
        else if (animatorStateInfo.IsName("PunchLeft") || animatorStateInfo.IsName("PunchRight"))
        {

            if (!isAttacking && animatorStateInfo.normalizedTime > 0.52f)
            {
                isAttacking = true;
                Attack();

            }
            
        }

        if (animatorStateInfo.IsName("Idle"))
        {
            isAttacking = false;

        }


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

    public void Attack()
    {
        Debug.Log("Attack start");

        LayerMask layerMask = LayerMask.GetMask(new string[] { "Enemy", "Resource" });

        Vector3 origin = transform.position + transform.forward * 0.4f + transform.up * 2f;
        var hits = Physics.SphereCastAll(origin, 0.2f, Vector3.down, 2f, layerMask);

        foreach (RaycastHit hit in hits)
        {

           /* Debug.Log($"{hit.transform.name}: point={hit.point}, distance={hit.distance},");
            Debug.DrawLine(origin, hit.point, Color.red, 1.0f);
            Debug.DrawRay(hit.point, hit.normal, Color.magenta, 1.0f);*/
            if (hit.collider.TryGetComponent(out Resource resource)) // 캘 수 있는 자원이라면 캐기
            {
                resource.Gather(gameObject.transform, equipmentController.GetQuantityPerHit()); // 한 번에 캘 수 있는 개수, 장비 장착시 달라지도록
            }
            // 적이라면 적에게 데미지
            float weaponDamage = equipmentController.GetDamage();
            if(weaponDamage == -1f) // -1을 리턴했다는 것은 장착한 무기가 없다는 뜻
            {
                // 따라서 여기서는 플레이어 기본 공격력으로 적에게 데미지주기
                // enemy.TakeDamage(playerAttack);
            }
            else
            {
                // 여기서는 무기 공격력으로 적에게 데미지 주기
                // 기획에 따라 둘 중 선택하면 될듯 합니다
                // enemy.TakeDamage(playerAttack + weaponDamage);
                // enemy.TakeDamage(weaponDamage);

            }
        }
    }

    public void TryAttack()
    {
        animator.SetTrigger(AnimParam.Attack);
        
    }
    void SetEquipped(bool equipped)
    {
        animator.SetBool(AnimParam.IsEquipped, equipped);
    }
    public Transform GetDropTransform()
    {
        return dropTransform;
    }
}
