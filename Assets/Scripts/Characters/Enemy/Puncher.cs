using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Puncher : MonoBehaviour, IDamageable
{
    [SerializeField] float _stopDistance;
    [SerializeField] float _attackTimeout;

    //=====Status=====
    [SerializeField] StatusSO statusSO;
    public int MaxHP { get; private set; }
    public int CurrentHP { get; private set; }
    public int Damage { get; private set; }

    public GameObject Target;
    private Animator _animator;
    private NavMeshAgent _agent;
    private float _attackTimeoutDelta;

    public event Action OnDeath;
    public bool IsDead { get; private set; }

    //=====UI=====
    [SerializeField] Slider _hpBar; 


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
    }

    void Update()
    {
        Chase();
        _attackTimeoutDelta += Time.deltaTime;
    }

    private void Init()
    {
        MaxHP = statusSO.HP;
        CurrentHP = statusSO.HP;
        Damage = statusSO.Damage;
        IsDead = false;
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

    private void GiveDamage() // 애니메이션 이벤트
    {
        IDamageable target = Target.GetComponent<IDamageable>();
        target?.TakeDamage(Damage);
    }

    public void TakeDamage(int damage)
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
        OnDeath?.Invoke();
        _animator.SetBool(PuncherAnimatorHashes.Dead, true);
        _agent.isStopped = true;
    }
}
