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
    private Vector3 _targetedPosition;
    public Camera MainCamera;
    public GameObject AimCamera => _aimCamera;

    //=====Weapon=====
    public int MaxAmmo { get; private set; } = 300;
    public int CurrentAmmo { get; set; }
    public Weapon Weapon { get; private set; }

    //=====Animation Rigging=====
    private RigBuilder _rigBuilder;
    private Dictionary<string, Rig> _rigLayersDictionanry = new Dictionary<string, Rig>();
    public GameObject TargetObject { get; set; }

    //=====Status=====
    [SerializeField] StatusSO statusSO;
    public int MaxHP { get; private set; }
    public int CurrentHP { get; private set; }
    public int Damage { get; private set; }
    public bool IsDead {  get; private set; }
    public event Action OnDeath;

    //=====States=====
    public bool IsRunning => Controller.IsRunning;
    public bool IsAiming { get; set; } = false;
    public bool IsReloading { get; set; } = false;
    public bool IsGround => Controller.IsGround;
    public bool IsFalling => Controller.IsFalling;

    //=====References=====
    public Animator Animator { get; private set; }
    public PlayerInputs Input { get; private set; }
    public PlayerController Controller { get; private set; }


    //=====FSM=====
    public StateMachine stateMachine { get; private set; }


    private void Awake()
    {
        Input = GetComponent<PlayerInputs>();
        Controller = GetComponent<PlayerController>();
        if(_weaponObject != null)
            Weapon = _weaponObject.GetComponent<Weapon>();
        Animator = GetComponent<Animator>();
        _rigBuilder = GetComponent<RigBuilder>();
        stateMachine = GetComponent<StateMachine>();
        RigInit();
    }

    void Start()
    {
        MainCamera = Camera.main;
        _aimCamera.SetActive(false);
        
        
        CurrentAmmo = MaxAmmo;

        InitStatus();
    }

    void Update()
    {
        if (IsDead) return;

        //CheckRun();
        /*if (Input.Reload)
            TryReloading();*/
        //Aim();
    }

    private void InitStatus()
    {
        MaxHP = statusSO.HP;
        CurrentHP = statusSO.HP;
        Damage = statusSO.Damage;
    }

    private void Aim()
    {
        if (Input.Aim && !IsReloading && !IsRunning)
        {
            AimControll(true);

            Transform camera = MainCamera.transform;
            if (Physics.Raycast(camera.position, camera.forward, out RaycastHit hit, 1000))
            {
                _targetedPosition = hit.point;
            }

            Vector3 tempVector = _targetedPosition;
            tempVector.y = transform.position.y;
            Vector3 playerDirection = (tempVector - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, playerDirection, Time.deltaTime * 20f);

            if (Input.Fire && !IsReloading)
            {
                Weapon.Fire(_targetedPosition);
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
            if (!IsReloading)
                SetRigWeight("RigHandLayer"); // 장전중일 때 리깅되면 부자연스럽
        }
        else
        {
            SetRigWeight("RigUpperBodyLayer", 0.0f);
            SetRigWeight("RigHandLayer", 0.0f);
        }
    }

    public void Reload()// 애니메이션 이벤트 등록
    {
        if(CurrentAmmo >= Weapon.AmmoCapacity) // 잔탄 넉넉
        {
            Weapon.CurrentAmmo = Weapon.AmmoCapacity;
            CurrentAmmo -= Weapon.AmmoCapacity;
        }
        else if(CurrentAmmo < Weapon.AmmoCapacity && CurrentAmmo > 0) // 잔탄 소량
        {
            Weapon.CurrentAmmo = CurrentAmmo;
            CurrentAmmo = 0;
        }

        Debug.Log($"플레이어 현재 잔탄: {CurrentAmmo}");

        IsReloading = false;
    }

    private void RigInit() // 딕셔너리에 이름이랑 리그레이어
    {
        TargetObject = new GameObject("TargetObject");

        foreach(RigLayer rigLayer in _rigBuilder.layers)
        {
            if(null != rigLayer.rig)
            {
                _rigLayersDictionanry[rigLayer.name] = rigLayer.rig;

                MultiAimConstraint multiAimConstraint = rigLayer.rig.GetComponentInChildren<MultiAimConstraint>();
                if(null != multiAimConstraint)
                {
                    WeightedTransformArray sourceObject = multiAimConstraint.data.sourceObjects;
                    sourceObject.Add(new WeightedTransform(TargetObject.transform, 1.0f));
                    multiAimConstraint.data.sourceObjects = sourceObject;
                }
            }
            else
                Debug.LogWarning($"Rig '{rigLayer.name}' is null.");
        }
    }

    public void SetRigWeight(string rigLayerName, float weight = 1.0f)
    {
        _rigLayersDictionanry.TryGetValue(rigLayerName, out Rig rig);
        if(null != rig)
            rig.weight = Mathf.Lerp(rig.weight, weight, Time.deltaTime * 20f);
        else
            Debug.LogWarning($"Rig '{rig.name}' is null.");
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


    // 달리면 장전 애니메이션, 장전 상태 다 제한 걸어야 해서 만듦
    /*private void CheckRun() // TODO: 진짜 조정 필요.
    {
        if(Input.Run) // 헷갈려서 나눠놓긴 했는데, 그냥 인풋런 값으로 대입해도 괜찮긴하다.
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
    }*/
}
