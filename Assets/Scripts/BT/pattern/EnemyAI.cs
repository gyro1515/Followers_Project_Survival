using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
class EnemyAI : MonoBehaviour
{
    #region Constructor
    [Header("Range")]
    [SerializeField] private float detectionRange = 10f; // 감지 범위
    [SerializeField] private float detectionAngle = 180f; // 감지 각도
    [SerializeField] private float AttackRange = 2.5f; // 공격 범위
    [SerializeField] private float returnDistance = 50f; // 돌아갈 거리
    [SerializeField] private float YieldRange = 10f; // idle 상태에서 랜덤으로 움직이는 범위
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f; // 이동 속도
    [Header("MinDPS")]
    [SerializeField] private float minDamageThreshold = 0.002f; // DPS
    

    Vector3 _originPos = default;// 원래 자리
    Vector3 _IdlePos = default; // idle 상태에서 랜덤으로 움직이는 위치
    bool _isRecover = false; // 회복 중에 플레이어 추적 방지용
    bool _isDead = false; // 죽었는지 여부
    BehaviourTreeRunner _BTRunner = null;// 행동 트리 실행기
    Transform _detectedPlayer = null;
    Animator _animator = null;
    Stopwatch _chaseTimer = new Stopwatch(); // 추적 시간 측정
    Stopwatch _idleTimer = new Stopwatch(); // idle 시간 측정
    NavMeshAgent _navMeshAgent = null; // 네비게이션 에이전트
    PlayerStatComponent _player; // 플레이어 체력 상호작용을 위한 변수
    MonsterStatComponent _enemy; // 자신 체력 상호작용을 위한 변수

    const string _ATTACK_ANIM_STATE_NAME = "Attack5";
    const string _ATTACK_ANIM_TRIGGER_NAME = "Attack5";
    const string _IDLE_ANIM_BOOL_NAME = "Idle";
    const string _RUN_ANIM_BOOL_NAME = "Run Forward";
    const string _WALK_ANIM_STATE_NAME = "Walk Forward";
    const string _WALK_ANIM_BOOL_NAME = "WalkForward";
    const string _DAMAGE_ANIM_STATE_NAME = "HitFront";
    const string _DAMAGE_ANIM_TRIGGER_NAME = "HitFront";
    const string _DEATH_ANIM_STATE_NAME = "Death";
    const string _DEATH_ANIM_BOOL_NAME = "Death";
    #endregion
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _BTRunner = new BehaviourTreeRunner(SettingBT());
        _originPos = transform.position;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _enemy = GetComponent<MonsterStatComponent>();
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
                new ActionNode(WasDead),
                new ActionNode(Death), // DeathAction
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
                                new ActionNode(MoveToPlayer)
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
                new SequenceNode( // IdleAction
                    new List<INode>
                    {
                        new ActionNode(IdleAction),
                        new ActionNode(IdleMove),
                        new ActionNode(IdleFixed)
                    }
                    )
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
                return (normalizedTime != 0 && normalizedTime < 1f);
            }
        }
        return false;
    }

    IEnumerator Recover()
    {
        while (_enemy.statValues[StatType.Health].BaseValue < _enemy.statValues[StatType.Health].MaxValue)
        {
            _isRecover = true;
            _chaseTimer.Stop();
            _chaseTimer.Reset();
            _enemy.statValues[StatType.Health].BaseValue += Math.Max(1, (_enemy.statValues[StatType.Health].MaxValue - _enemy.statValues[StatType.Health].BaseValue) * 0.3f);
            if (_enemy.statValues[StatType.Health].BaseValue > _enemy.statValues[StatType.Health].MaxValue)
            {
                _enemy.statValues[StatType.Health].BaseValue = _enemy.statValues[StatType.Health].MaxValue;
            }
            yield return new WaitForSeconds(0.1f);
        }
        _isRecover = false;

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
            if(IsAnimationRunning(_ATTACK_ANIM_STATE_NAME)) 
            {
                return INode.ENodeState.Running;
            }
            else 
            { 
                _animator.SetBool(_WALK_ANIM_BOOL_NAME, false);
                _animator.SetBool(_RUN_ANIM_BOOL_NAME, false);
                _animator.SetBool(_IDLE_ANIM_BOOL_NAME, false);
                _animator.SetTrigger(_ATTACK_ANIM_TRIGGER_NAME);
            }
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
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, returnDistance);
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
                _player = hitCollider.GetComponent<PlayerStatComponent>();
                _detectedPlayer = hitCollider.transform;
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
        if (_enemy.statValues[StatType.Health].BaseValue < _enemy.statValues[StatType.Health].MaxValue - 1 && !_isRecover)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    _player = hitCollider.GetComponent<PlayerStatComponent>();
                    _detectedPlayer = hitCollider.transform;
                    Vector3 directionToPlayer = (hitCollider.transform.position - transform.position).normalized;
                    float angle = Vector3.Angle(transform.forward, directionToPlayer);
                    _detectedPlayer = hitCollider.transform;
                    return INode.ENodeState.Success;
                }
            }
            _detectedPlayer = null;
            return INode.ENodeState.Failure;
        }
        return INode.ENodeState.Failure;    
    }

    INode.ENodeState MoveToPlayer()
    {
        //NevMesh 를 이용한 이동 구현
        // 이동이 불가능할 때 실패(경로가 나오지 않을 때)
        if(IsAnimationRunning(_ATTACK_ANIM_STATE_NAME) && _detectedPlayer != null)
        {
            return INode.ENodeState.Failure; // 공격 애니메이션이 실행 중일 때는 이동 불가
        }

        _chaseTimer.Start();
        _animator.SetBool(_IDLE_ANIM_BOOL_NAME, false);
        _animator.SetBool(_RUN_ANIM_BOOL_NAME, true);
        _animator.SetBool(_WALK_ANIM_BOOL_NAME, false);
        if (_navMeshAgent.SetDestination(_detectedPlayer.position))
        {
            _navMeshAgent.speed = moveSpeed;
            _navMeshAgent.isStopped = false;
            return INode.ENodeState.Running;
        }
        else if (Vector3.SqrMagnitude(_detectedPlayer.position - transform.position) < (AttackRange * AttackRange))
        {
            // 플레이어가 공격 범위 내에 들어왔을 때
            _navMeshAgent.isStopped = true;
            return INode.ENodeState.Success;
        }
        else
        {
            _navMeshAgent.isStopped = true;
            return INode.ENodeState.Failure;
        }
    }


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
        if (_chaseTimer.Elapsed.Seconds >= 60)
        { 
            if (minDamageThreshold >= (_enemy.statValues[StatType.Health].MaxValue - _enemy.statValues[StatType.Health].BaseValue) * 1000 / _chaseTimer.Elapsed.Seconds)
            {
                return INode.ENodeState.Success;
            }
        }
        return INode.ENodeState.Failure;
    }

    INode.ENodeState MoveToOrigin()
    {
        _detectedPlayer = null;
        // 원래 자리로 이동: Nevimesh 이용?
        if (_navMeshAgent.SetDestination(_originPos))
        {
            _navMeshAgent.speed = moveSpeed;
            _navMeshAgent.isStopped = false;
            _animator.SetBool(_IDLE_ANIM_BOOL_NAME, false);
            _animator.SetBool(_RUN_ANIM_BOOL_NAME, true);
            return INode.ENodeState.Running;
        }
        else if (Vector3.SqrMagnitude(_originPos - transform.position) < (returnDistance * returnDistance))
        {
            // 원래 자리로 이동했을 때
            _navMeshAgent.isStopped = true;
            _animator.SetBool(_RUN_ANIM_BOOL_NAME, false);
            _animator.SetBool(_IDLE_ANIM_BOOL_NAME, true);
        }
        else
        {
            _navMeshAgent.isStopped = true;
            return INode.ENodeState.Failure;
        }
        return INode.ENodeState.Success;
    }

    INode.ENodeState RecoverHealth()
    {
        StartCoroutine(Recover());
        return INode.ENodeState.Success;
    }

    INode.ENodeState IdleAction()
    {
        if (IsAnimationRunning(_ATTACK_ANIM_STATE_NAME))
        {
            return INode.ENodeState.Failure; // 전투중 자꾸 애니메이션이 취소됨
        }
        // idle 상태로 판정
        if (_idleTimer.Elapsed.Seconds <= 1.5f)
        {
            _animator.SetBool(_IDLE_ANIM_BOOL_NAME, true);
            _animator.SetBool(_RUN_ANIM_BOOL_NAME, false);
            _animator.SetBool(_WALK_ANIM_BOOL_NAME, false);
            _navMeshAgent.isStopped = true;
        }
        else if (UnityEngine.Random.Range(0, 2) == 0)
        {
            _idleTimer.Reset();
            if (!IsAnimationRunning(_WALK_ANIM_STATE_NAME))
            {
                _animator.SetBool(_IDLE_ANIM_BOOL_NAME, false);
                _animator.SetBool(_RUN_ANIM_BOOL_NAME, false);
                _animator.SetBool(_WALK_ANIM_BOOL_NAME, true);
                _IdlePos = UnityEngine.Random.insideUnitSphere * YieldRange;
                _IdlePos += transform.position;
            }
        }
        else
        {
            _animator.SetBool(_IDLE_ANIM_BOOL_NAME, true);
            _animator.SetBool(_RUN_ANIM_BOOL_NAME, false);
            _animator.SetBool(_WALK_ANIM_BOOL_NAME, false);
            _navMeshAgent.isStopped = true;
        }
            return INode.ENodeState.Success;
    }
    INode.ENodeState IdleMove()
    {
        if (_idleTimer.IsRunning) // fixed 상태일때 Failure 반환
        {
            return INode.ENodeState.Failure;
        }
        if (IsAnimationRunning(_ATTACK_ANIM_STATE_NAME))
        {
            return INode.ENodeState.Failure; // 전투중 애니메이션 취소 방지
        }
        _navMeshAgent.isStopped = true;
        // 랜덤한 위치로 이동
        NavMeshHit hit;
        if (NavMesh.SamplePosition(_IdlePos, out hit, YieldRange, NavMesh.AllAreas))
        {
            _IdlePos = hit.position;
            if (_navMeshAgent.SetDestination(_IdlePos))
            {
                _navMeshAgent.speed = moveSpeed;
                _navMeshAgent.isStopped = false;
                return INode.ENodeState.Success;
            }
            else if (Vector3.SqrMagnitude(_IdlePos - transform.position) < 6)
            {
                // 목표에 근접한 위치로 이동했을 때
                _navMeshAgent.isStopped = true;
                _animator.SetBool(_IDLE_ANIM_BOOL_NAME, true);
                return INode.ENodeState.Success;
            }
        }
        
        return INode.ENodeState.Failure;
    }
    INode.ENodeState IdleFixed()
    {
        if(IsAnimationRunning(_WALK_ANIM_STATE_NAME))
        {
            return INode.ENodeState.Failure;
        }
        
        if(_idleTimer.Elapsed.TotalSeconds == 0)
        {
            _idleTimer.Start();
        }
        _animator.SetBool(_IDLE_ANIM_BOOL_NAME, true);
        return INode.ENodeState.Success;
    }

    INode.ENodeState Death()
    {
        if (_enemy.statValues[StatType.Health].BaseValue <= 0)
        {
            _animator.SetBool(_DEATH_ANIM_BOOL_NAME, true);
            _navMeshAgent.isStopped = true;
            _navMeshAgent.enabled = false;
            _isDead = true;
            return INode.ENodeState.Success;
        }
        return INode.ENodeState.Failure;
    }

    INode.ENodeState WasDead()
    {
        if (_isDead)
        {
            _animator.SetBool("End", true); // 죽었을 때 애니메이션 종료
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime == 1f)
                _animator.StopPlayback(); // 애니메이션이 끝났을 때 재생 중지
            return INode.ENodeState.Success; // 죽었을 때는 더 이상 행동하지 않음
        }
        return INode.ENodeState.Failure; // 죽지 않았을 때는 행동 트리 계속 진행
    }
    #endregion
}
