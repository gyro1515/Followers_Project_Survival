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
using System.Collections;
using System.Diagnostics;

[RequireComponent(typeof(Animator))]
class EnemyAI : MonoBehaviour
{
    #region Constructor
    [Header("Range")]
    [SerializeField] private float detectionRange = 10f; // 감지 범위
    [SerializeField] private float detectionAngle = 180f; // 감지 각도
    [SerializeField] private float AttackRange = 2f; // 공격 범위
    [SerializeField] private float returnDistance = 50f; // 돌아갈 거리
    [SerializeField] private float YieldRange = 10f; // idle 상태에서 랜덤으로 움직이는 범위
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f; // 이동 속도
    [Header("MinDPS")]
    [SerializeField] private float minDamageThreshold = 0.002f; // DPS
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f; // 최대 체력

    Vector3 _originPos = default;// 원래 자리
    float _currentHealth = 100f; // 현재 체력
    BehaviourTreeRunner _BTRunner = null;// 행동 트리 실행기
    Transform _detectedPlayer = null;
    Animator _animator = null;
    bool _isChaseActive = false; // 추적 상태
    Stopwatch _chaseTimer = new Stopwatch(); // 추적 시간 측정

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
                        //new ActionNode(isChaseActive),
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

    IEnumerator Recover()
    {
        yield return new WaitForSeconds(0.5f);
        _currentHealth += (maxHealth - _currentHealth) * 0.1f;
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

    INode.ENodeState EndChase()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 20f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                return INode.ENodeState.Success;
            }
        }
        return INode.ENodeState.Failure;
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

    INode.ENodeState DamageDetect()
    {
        // 플레이어로부터 공격을 받았을 때
        return INode.ENodeState.Success;
    }

    INode.ENodeState MoveToPlayer()
    {
        _isChaseActive = true;
        _chaseTimer.Restart();
        //NevMesh 를 이용한 이동 구현
        // 이동이 불가능할 때 실패(경로가 나오지 않을 때)
        return INode.ENodeState.Success;
    }

    INode.ENodeState CoolDownChase()
    {
        // 플레이어를 계속 추적하도록 한동안 Running 상태 유지.
        // 공격 범위 내에 들어왔을 때에 Running 상태 종료하고 Success 반환.
        return INode.ENodeState.Running;
        // 그런데 생각이 드는 게 이 프로세스가 실행되고 있는 동안에는 AI의 트리 서칭이 멈추고 있는 게 아닌가.
    }

    //INode.ENodeState isChaseActive()
    //{
    //    if (_isChaseActive)
    //    {
    //        return INode.ENodeState.Failure;
    //    }
    //    return INode.ENodeState.Success;
    //}

    INode.ENodeState DistanceCheck()
    {
        if (Vector3.SqrMagnitude(_originPos - transform.position) > (returnDistance * returnDistance))
        {
            return INode.ENodeState.Success;
        }
        return INode.ENodeState.Failure;
    }

    INode.ENodeState DPSCheck()
    {
        if (minDamageThreshold >= (maxHealth-_currentHealth)*1000/_chaseTimer.Elapsed.TotalSeconds)
        {
            return INode.ENodeState.Success;
        }
        return INode.ENodeState.Failure;
    }

    INode.ENodeState MoveToOrigin()
    {
        _isChaseActive = false;
        _detectedPlayer = null;
        // 원래 자리로 이동: Nevimesh 이용?
        return INode.ENodeState.Success;
    }

    INode.ENodeState RecoverHealth()
    {
        StartCoroutine(Recover());
        return INode.ENodeState.Success;
    }

    INode.ENodeState Idle()
    {
        // 랜덤으로 2개의 행동 패턴 중 하나를 선택해 실행: 기다리기, 움직이기
        // 기다리기: 움직이지 않고 idle 애니메이션 실행. 애니메이션 실행 시간은 최대 1초.
        // 대기 중 적 감지 시 idle 상태 즉시 종료 후 추적으로 변환
        // 움직이기: 랜덤한 방향으로 몸을 틀고 서식지 내의 랜덤 좌표로 이동: NevMesh 사용
        // 움직임이 불가능한 좌표일 경우 강제로 기다리기 상태로 전환.
        return INode.ENodeState.Success;
    }
    #endregion
}
