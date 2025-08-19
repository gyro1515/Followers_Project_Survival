///<LayOut>
///
///  Idle: 랜덤으로 움직이고, 행동 방향을 바꿈.
///  
///  감지 범위는 시야 방향을 기준으로 180도, 거리는 10m. 
///  혹은 공격 판정을 받았을 경우(플레이어로부터의 공격을 받았을 때).
/// 
///  감지 후에는 플레이어 추적.
///  
///  기본 행동 반경으로부터 50m 밖으로 벗어나고, 120초 이내에 유의미한 피해(최대 체력의 25%)가 들어오지 않으면 원래 자리로 돌아가 체력을 전부 회복함.
/// 
///  플레이어가 죽었을 때에도 원래 자리로 돌아가 체력을 전부 회복함.
/// 
/// 
/// </LayOut>
/// 
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

[RequireComponent(typeof(Animator))]
class EnemyAI : MonoBehaviour
{
    #region Constructor
    [Header("Range")]
    [SerializeField] private float detectionRange = 10f; // 감지 범위
    [SerializeField] private float detectionAngle = 180f; // 감지 각도
    [SerializeField] private float AttackRange = 2f; // 공격 범위
    [SerializeField] private float returnDistance = 50f; // 돌아갈 거리
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f; // 이동 속도
    [Header("MinDPS")]
    [SerializeField] private float maxDamageThreshold = 0.002f; // DPS
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f; // 최대 체력

    Vector3 _originPos = default;// 원래 자리
    float _currentHealth = 100f; // 현재 체력
    BehaviourTreeRunner _BTRunner = null;// 행동 트리 실행기
    Transform _detectedPlayer = null;
    Animator _animator = null;

    const string _ATTACK_ANIM_STATE_NAME = "Attack";
    const string _ATTACK_ANIM_TRIGGER_NAME = "attack";
    #endregion
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _BTRunner = new BehaviourTreeRunner(SettingBT());
        _originPos = transform.position;
    }

    private void Update()
    {
        _BTRunner.Operate();
    }
    INode SettingBT()
    {
        // Define the behavior tree structure
        return new SelectorNode( //rootsel
            new List<INode>
            { 
                new SequenceNode( //AttackSeq
                new List<INode>
                {
                    new ActionNode(CanAttack), 
                    new ActionNode(Attack),
                    new ActionNode(AttackCooldown) 
                }
            ),
                new SequenceNode( // ChaseSeq
                    new List<INode>
                    {
                        new ActionNode(EndChase),
                        new SelectorNode( // DetectSel
                            new List<INode>
                            {
                                new ActionNode(RangeDetect),
                                new ActionNode(DamageDetect)
                            }
                            ),
                        new SequenceNode( // moveseq
                            new List<INode>
                            {
                                new ActionNode(MoveToPlayer),
                                new ActionNode(CoolDownChase)
                            }
                            )
                    }
                    ),
                new SequenceNode( //ReturnSeq
                    new List<INode>
                    {
                        new ActionNode(isChaseActive),
                        new SelectorNode( // ReturnSel
                            new List<INode>
                            {
                                new ActionNode(DistanceCheck),
                                new ActionNode(DPSCheck)
                            }
                            ),
                        new ActionNode(MoveToOrigin),
                        new ActionNode(RecoverHealth)
                    }
                    ),
                new ActionNode(Idle) // IdleAction
            
            
            }

        );
    }

    bool IsAnimationRunning(string stateName)
    {
        if (_animator != null)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            {
                var normalizedTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                return normalizedTime != 0 && normalizedTime < 1f;
            }
        }
        return false;
    }
    #region Actions
    INode.ENodeState CanAttack()
    {
        if (IsAnimationRunning(_ATTACK_ANIM_STATE_NAME))
        {
            return INode.ENodeState.Failure;
        }
        else if ( _detectedPlayer != null &&
            Vector3.SqrMagnitude(_detectedPlayer.position - transform.position) < (AttackRange*AttackRange))
        {
            return INode.ENodeState.Success;
        }
        return INode.ENodeState.Failure;
    }

    INode.ENodeState Attack()
    {
        if ( _detectedPlayer != null)
        {
            _animator.SetTrigger(_ATTACK_ANIM_TRIGGER_NAME);
            return INode.ENodeState.Success;
        }
        return INode.ENodeState.Failure;
    }

    INode.ENodeState AttackCooldown()
    {
        if (IsAnimationRunning(_ATTACK_ANIM_STATE_NAME))
        {
            return INode.ENodeState.Running;
        }
        return INode.ENodeState.Success;
    }

    INode.ENodeState RangeDetect()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Vector3 directionToPlayer = (hitCollider.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, directionToPlayer);
                if (angle < detectionAngle / 2f)
                {
                    _detectedPlayer = hitCollider.transform;
                    return INode.ENodeState.Success;
                }
            }
        }
        _detectedPlayer = null;
        return INode.ENodeState.Failure;
    }

    #endregion
}
