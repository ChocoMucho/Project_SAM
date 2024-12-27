using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Turret : MonoBehaviour, IDamageable
{
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _fireTimeout;
    [SerializeField] private Transform _turretCenter;
    [SerializeField] private Transform _muzzle;
    [SerializeField] private LayerMask layerMask = new LayerMask();
    [SerializeField] GameObject _impactEffect;
    [SerializeField] GameObject _muzzleEffect;
    [SerializeField] float _detectRange;

    //=====Status=====
    [SerializeField] StatusSO statusSO;
    public int MaxHP { get; private set; }
    public int CurrentHP { get; private set; }
    public int Damage { get; private set; }

    public GameObject Target;
    private float _fireTimeoutDelta;
    private Vector3 _lookPoint; // 일단 총알 도달할 곳

    public event Action OnDeath;
    public bool IsDead { get; private set; }

    //=====UI=====
    [SerializeField] Slider _hpBar;

    void Start()
    {
        Init();
    }

    void Update()
    {
        _fireTimeoutDelta += Time.deltaTime;
        Look();
    }

    private void Init()
    {
        MaxHP = statusSO.HP;
        CurrentHP = statusSO.HP;
        Damage = statusSO.Damage;
        IsDead = false;
        _hpBar.value = (float)CurrentHP / MaxHP;
    }
    private void Look()
    {
        Vector3 direction;

        if (IsDead || Vector3.Distance(transform.position, Target.transform.position) >= _detectRange)
        {
            direction = Vector3.down;
            Quaternion barrelRotation = Quaternion.LookRotation(direction);
            _turretCenter.rotation = Quaternion.Lerp(_turretCenter.rotation, barrelRotation, Time.deltaTime * _rotationSpeed);
        }
        else
        {
            // 타겟 방향 체크
            direction = (Target.transform.position - _turretCenter.position + new Vector3(0, 1, 0)).normalized;
            // 회전
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            _turretCenter.rotation = Quaternion.Lerp(_turretCenter.rotation, targetRotation, Time.deltaTime * _rotationSpeed);

            float angle = Vector3.Angle(_turretCenter.forward, direction);
            if (angle < 7f)
                Fire();
        }
    }

    private void Fire()
    {
        if (_fireTimeoutDelta >= _fireTimeout)
        {
            Instantiate(_muzzleEffect, _muzzle.position, Quaternion.identity);
            if ((Physics.Raycast(_muzzle.position, _muzzle.forward, out RaycastHit hit, 1000)))
            {
                Instantiate(_impactEffect, hit.point, Quaternion.identity);
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    IDamageable target = hit.collider.GetComponent<IDamageable>();
                    target?.TakeDamage(Damage);
                }
            }

            _fireTimeoutDelta = 0f;
        }     
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
        //Debug.Log($"터렛 {damage}만큼 데미지 받음");
    }

    private void DeadExecute()
    {
        IsDead = true;
        CurrentHP = 0;
        OnDeath?.Invoke();
    }
}
