using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class Player : MonoBehaviour, IDamageable
{
    //=====Camera=====
    [SerializeField] private GameObject _aimCamera;
    [SerializeField] private LayerMask layerMask = new LayerMask();
    [SerializeField] private Transform _cameraRoot;
    [SerializeField] private GameObject _weaponObject;
    [SerializeField] private Transform _aimDebugObject;
    private Vector3 _targetedPosition;
    private Camera _mainCamera;

    //=====Weapon=====
    public int MaxAmmo { get; private set; } = 300;
    public int CurrentAmmo { get; set; }

    //=====Animation Rigging=====
    private RigBuilder _rigBuilder;
    private Dictionary<string, Rig> _rigLayersDictionanry = new Dictionary<string, Rig>();

    //=====Status=====
    [SerializeField] StatusSO statusSO;
    public int MaxHP { get; private set; }
    public int CurrentHP { get; private set; }
    public int Damage { get; private set; }
    public bool IsDead {  get; private set; }
    public event Action OnDeath;

    //=====States=====
    public bool IsAiming { get; set; } = false;
    public bool IsReloading { get; set; } = false;
    public bool IsRunning { get; private set; }

    //=====References=====
    public Animator Animator { get; private set; }
    private PlayerInputs _input;
    private PlayerController _controller;
    private Weapon _weapon;

    //=====UI=====
    [SerializeField] private Image _hpBar;
    [SerializeField] private Text _ammo;

    private void Awake()
    {
        _input = GetComponent<PlayerInputs>();
        _controller = GetComponent<PlayerController>();
        if(_weaponObject != null)
            _weapon = _weaponObject.GetComponent<Weapon>();
        Animator = GetComponent<Animator>();

        _rigBuilder = GetComponent<RigBuilder>();
    }

    void Start()
    {
        _mainCamera = Camera.main;
        _aimCamera.SetActive(false);
        RigInit();

        CurrentAmmo = MaxAmmo;

        InitStatus();
    }

    void Update()
    {
        UpdateUI();
        if (IsDead) return;

        CheckRun();
        if (_input.reload)
            TryReloading();
        Aim();
    }

    private void InitStatus()
    {
        MaxHP = statusSO.HP;
        CurrentHP = statusSO.HP;
        Damage = statusSO.Damage;
    }

    private void Aim()
    {
        if (_input.aim && !IsReloading && !IsRunning)
        {
            AimControll(true);

            Transform camera = _mainCamera.transform;
            if (Physics.Raycast(camera.position, camera.forward, out RaycastHit hit, 1000))
            {
                _targetedPosition = hit.point;
                _aimDebugObject.position = _targetedPosition;
            }

            Vector3 tempVector = _targetedPosition;
            tempVector.y = transform.position.y;
            Vector3 playerDirection = (tempVector - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, playerDirection, Time.deltaTime * 20f);

            if(_input.fire && !IsReloading)
            {
                _weapon.Fire(_targetedPosition);
            }           
        }
        else
        {
            AimControll(false);
        }
    }

    private void AimControll(bool isAim)
    {
        IsAiming = isAim;
        _aimCamera.SetActive(isAim);

        float layerWeight = Animator.GetLayerWeight(1);
        if (isAim)
        {
            if (layerWeight >= 0.9f)
                layerWeight = 1.0f;
            Animator.SetLayerWeight(1, Mathf.Lerp(layerWeight, 1, Time.deltaTime * 10f));
            SetRigWeight("RigUpperBodyLayer");
            if(IsReloading)
                SetRigWeight("RigHandLayer", 0f); // 장전중일 때 리깅되면 부자연스럽
            else
                SetRigWeight("RigHandLayer"); 
        }
        else
        {
            if(IsReloading)
                Animator.SetLayerWeight(1, Mathf.Lerp(layerWeight, 1, Time.deltaTime * 10f));
            else
                Animator.SetLayerWeight(1, Mathf.Lerp(layerWeight, 0, Time.deltaTime * 10f));
            SetRigWeight("RigUpperBodyLayer", 0.0f);
            SetRigWeight("RigHandLayer", 0.0f);
        }
    }

    public bool TryReloading()
    {
        _input.reload = false;

        if (IsReloading)
            return false;

        if (CurrentAmmo <= 0)
            return false;
        else
        {
            Animator.SetTrigger(PlayerAnimatorHashes.Reload);
            IsReloading = true;
        }

        return true;
    }

    public void Reload()// 애니메이션 이벤트 등록
    {
        if(CurrentAmmo >= _weapon.AmmoCapacity) // 잔탄 넉넉
        {
            _weapon.CurrentAmmo = _weapon.AmmoCapacity;
            CurrentAmmo -= _weapon.AmmoCapacity;
        }
        else if(CurrentAmmo < _weapon.AmmoCapacity && CurrentAmmo > 0) // 잔탄 소량
        {
            _weapon.CurrentAmmo = CurrentAmmo;
            CurrentAmmo = 0;
        }

        Debug.Log($"플레이어 현재 잔탄: {CurrentAmmo}");

        IsReloading = false;
    }

    private void RigInit() // 딕셔너리에 이름이랑 리그레이어
    {
        foreach(RigLayer rigLayer in _rigBuilder.layers)
        {
            _rigLayersDictionanry[rigLayer.name] = rigLayer.rig;
        }        
    }

    private void SetRigWeight(string rigLayerName, float weight = 1.0f)
    {
        _rigLayersDictionanry.TryGetValue(rigLayerName, out Rig rig);
        rig.weight = Mathf.Lerp(rig.weight, weight, Time.deltaTime * 20f);
    }

    public void TakeDamage(int damage)
    {
        if (!IsDead)
        {
            CurrentHP -= damage;
            
            if (CurrentHP <= 0)
            {
                IsDead = true;
                CurrentHP = 0;
                OnDeath?.Invoke();
                Animator.SetBool(PlayerAnimatorHashes.Dead, IsDead);
            }
        }
        else
            return;
    }

    private void UpdateUI()
    {
        _hpBar.fillAmount = (float)CurrentHP / MaxHP;
        _ammo.text = $"{_weapon.CurrentAmmo} / {CurrentAmmo}";
    }

    // 달리면 장전 애니메이션, 장전 상태 다 제한 걸어야 해서 만듦
    private void CheckRun() // TODO: 진짜 조정 필요.
    {
        if(_input.run) // 헷갈려서 나눠놓긴 했는데, 그냥 인풋런 값으로 대입해도 괜찮긴하다.
        {
            IsRunning = true;
            Animator.SetBool(PlayerAnimatorHashes.Run, true);
            IsReloading = false;
        }
        else
        {
            IsRunning = false;
            Animator.SetBool(PlayerAnimatorHashes.Run, false);
        }
    }
}
