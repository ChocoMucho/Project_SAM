using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Puncher : Enemy
{
    [SerializeField] public float _stopDistance;
    [SerializeField] public float _detectDistance;
    [SerializeField] float _attackTimeout;


    private Animator _animator;
    private NavMeshAgent _agent;
    private float _attackTimeoutDelta;


    //=====States=====
    public bool IsTargetDetected => _detectDistance >= Vector3.Distance(transform.position, Target.transform.position);
    public bool IsClose => _stopDistance >= Vector3.Distance(transform.position, Target.transform.position);

    //=====UI=====
    [SerializeField] Slider _hpBar; 


    //=====Behavior Tree=====
    [SerializeField] private List<Transform> patrolPoints;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        _agent.stoppingDistance = _stopDistance;      
    }

    void Start()
    {
        _attackTimeoutDelta += Time.deltaTime;

        Init();
        InitStatus();
        InitTree();
    }

    void Update()
    {
        Tree.Process();
        //Chase();
        CheckWalking();
        _attackTimeoutDelta += Time.deltaTime;
    }

    public override void InitStatus()
    {
        Tree = new BehaviorTree("Puncher");
        Selector rootSelector = new Selector("RootSelector");

        Sequence patrolSequence = new Sequence("PatrolSequence");
        patrolSequence.AddChild(new Leaf("NotDetect", new Condition(() => !IsTargetDetected)));
        patrolSequence.AddChild(new Leaf("Patrol", new PatrolStrategy(transform, _agent, patrolPoints)));
        rootSelector.AddChild(patrolSequence);

        // 추격
        Sequence chaseSequence = new Sequence("ChaseSequence");
        chaseSequence.AddChild(new Leaf("Detect", new Condition(() => IsTargetDetected)));
        chaseSequence.AddChild(new Leaf("NotClose", new Condition(() => !IsClose)));
        chaseSequence.AddChild(new Leaf("Chase", new ActionStrategy(() => _agent.SetDestination(Target.transform.position))));
        rootSelector.AddChild(chaseSequence);

        // 공격 시퀀스 추가
        Sequence attackSequence = new Sequence("AttackSequence");
        attackSequence.AddChild(new Leaf("Close", new Condition(() => IsClose)));
        attackSequence.AddChild(new Leaf("Attack", new ActionStrategy(Attack)));
        rootSelector.AddChild(attackSequence);

        Tree.AddChild(rootSelector);
    }
    private void CheckWalking()
    {
        _animator.SetFloat(PuncherAnimatorHashes.WalkBlend, _agent.velocity.magnitude);
    }

    private void Init()
    {
        _hpBar.value = (float)CurrentHP / MaxHP;
    }

    private void Chase()
    {
        if (IsDead) return;

        float distance = Vector3.Distance(transform.position, Target.transform.position);
        if(distance > _stopDistance)
        {
            _agent.SetDestination(Target.transform.position);
            _agent.isStopped = false;
            _animator.SetBool(PuncherAnimatorHashes.Chase, true);
        }
        else
        {
            _agent.isStopped = true;
            _animator.SetBool(PuncherAnimatorHashes.Chase, false);
            Attack();
        }
    }

    private void Attack()
    {
        if(_attackTimeout <= _attackTimeoutDelta)
        {
            _attackTimeoutDelta = 0f;
            _animator.SetTrigger(PuncherAnimatorHashes.Attack);
        }
    }

    private void GiveDamage() // 애니메이션에 달아놓은 이벤트
    {
        if(Vector3.Distance(transform.position, Target.transform.position) <= _stopDistance)
        {
            IDamageable target = Target.GetComponent<IDamageable>();
            target?.TakeDamage(Damage);
        }
    }

    public override void TakeDamage(int damage)
    {
        if (!IsDead)
        {
            CurrentHP -= damage;
            _hpBar.value = (float)CurrentHP / MaxHP;
            if (CurrentHP <= 0)
            {
                DeadExecute();
            }
        }
        else
            return;        
        //Debug.Log($"펀처 {damage}만큼 데미지 받음");
    }

    private void DeadExecute()
    {
        IsDead = true;
        CurrentHP = 0;
        TriggerOnDeath();
        _animator.SetBool(PuncherAnimatorHashes.Dead, true);
        _agent.isStopped = true;
    }
}
