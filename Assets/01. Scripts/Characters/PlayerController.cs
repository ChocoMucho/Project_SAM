using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Windows;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerController : MonoBehaviour
{
    //=====Player Move Values=====
    [field: SerializeField] public float WalkSpeed { get; private set; } = 2.5f;
    [field: SerializeField] public float RunSpeed { get; private set; } = 4.0f;
    [field: SerializeField] public float Gravity { get; set; } = -9.81f;
    [field: SerializeField] public float JumpHeight { get; private set; } = 1.2f;
    [field: SerializeField] public float RotationSmoothTime { get; private set; } = 0.1f;
    [field: SerializeField] public float SpeedChangeRate { get; private set; } = 10.0f;

    //=====Ground and Air=====
    [field: SerializeField] public bool IsGround { get; private set; } = false;
    [field: SerializeField] public float GroundedRadius { get; private set; }
    private float GroundedOffset = -0.14f;
    [field: SerializeField] public LayerMask GroundLayers;

    //=====Player Move Timeout=====
    [field: SerializeField] public float JumpTimeout { get; private set; } = 0.3f;
    [field: SerializeField] public float FallTimeout { get; private set; } = 0.1f;
    private float JumpTimeoutDelta; // 0이 돼야 점프 가능
    private float FallTimeoutDelta;

    //player move value
    private Vector3 _inputDirection;
    private float _verticalVelocity = 0f;
    private float _playerTargetYaw;
    private float _speed; // 현재 속도
    private float _playerRotationVelocity;
    private Vector3 _targetDirection;
    private float _animationBlend;
    private Vector2 _animationBlend2;

    //=====Camera=====
    [field: SerializeField] public Transform CameraRoot { get; private set; }
    [field: SerializeField] public float TopClamp { get; private set; } = 70.0f;
    [field: SerializeField] public float BottomClamp { get; private set; } = -30.0f;
    [field: SerializeField] public float Sensitivity { get; private set; } = 10f;
    private float _cameraTargetYaw;    // 횡 이동
    private float _cameraTargetPitch;  // 종 이동

    //=====Boost=====
    [field: SerializeField] public float BoostSpeed { get; private set; } = 20f;
    [field: SerializeField] public float BoostDuration { get; private set; } = 0.5f;
    private float _elapsedBoostTime;
    private bool _isBoosting = false;

    

    //=====References=====
    private Player _player;
    private GameObject _mainCamera;
    private CharacterController _controller;
    private PlayerInputs _input;
    private Animator _animator;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputs>();
        _animator = GetComponent<Animator>();
        BoostDuration = 0.5f;
    }

    void Start()
    {
        _cameraTargetYaw = CameraRoot.rotation.eulerAngles.y;        
        GroundedRadius = _controller.radius;    
    }

    void Update()
    {
        GroundCheck();
        if (_player.IsDead)
            return;
        JumpAndGravity();
        CapturePlayerDirection();
        Move();
        //Boost();
    }

    

    private void LateUpdate()
    {
        CameraRotation();
    }

    #region 카메라
    private void CameraRotation()
    {
        // 입력 값을 요우, 피치 값에 적용
        if(_input.look.magnitude >= 0.1f)
        {
            _cameraTargetYaw += _input.look.x * Time.deltaTime * Sensitivity;
            _cameraTargetPitch -= _input.look.y * Time.deltaTime * Sensitivity;
        }
        _cameraTargetYaw = ClampAngle(_cameraTargetYaw, -360f, 360f);
        _cameraTargetPitch = ClampAngle(_cameraTargetPitch, BottomClamp, TopClamp);

        CameraRoot.rotation = Quaternion.Euler(_cameraTargetPitch, _cameraTargetYaw, 0f);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        // -360~360 조정
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;

        return Mathf.Clamp(angle, min, max);
    }
    #endregion

    #region 점프, 공중
    private void GroundCheck()
    {
        // 밑에 캐릭터 컨트롤러랑 지름 똑같은 구체 만들어서 살짝 내리고 닿는지 체크
        Vector3 spherePosition =
            new Vector3(
                transform.position.x,
                transform.position.y - GroundedOffset,
                transform.position.z);
        IsGround = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);
    }

    private void JumpAndGravity()
    {
        if(IsGround)
        {
            FallTimeoutDelta = FallTimeout;

            if (IsGround && _verticalVelocity <= 0.0f)
                _verticalVelocity = -2f;

            if (_input.Jump && JumpTimeoutDelta <= 0f)
            {
                _verticalVelocity = Mathf.Sqrt(JumpHeight * 2 * -Gravity);
            }

            if(JumpTimeoutDelta >= 0.0f)
                JumpTimeoutDelta -= Time.deltaTime;
        }
        else
        {
            JumpTimeoutDelta = JumpTimeout;
            _input.Jump = false;
        }
        

        
        _verticalVelocity += Gravity * Time.deltaTime;
    }
    #endregion

    #region 움직임
    private void CapturePlayerDirection()
    {
        _inputDirection = new Vector3(_input.move.x, 0, _input.move.y).normalized;
        if (_input.move != Vector2.zero)
        {
            // 메인 카메라 yaw 값 + 입력의 yaw 값 -> Atan2 * Rad머시기 그거
            _playerTargetYaw = _mainCamera.transform.eulerAngles.y + (Mathf.Atan2(_inputDirection.x, _inputDirection.z) * Mathf.Rad2Deg);
        }

        // 오일러 * forward -> 원하는 벡터. 아무튼 됨
        _targetDirection = Quaternion.Euler(0f, _playerTargetYaw, 0f) * Vector3.forward;
    }

    private void Move()
    {
        float targetSpeed = WalkSpeed;
        if(_input.Run)
            targetSpeed = RunSpeed;

        float inputMagnitude = _input.move.magnitude;

        if (_input.move == Vector2.zero)
        {
            targetSpeed = 0;
        }
        else 
        {
            //회전
            float yaw = Mathf.SmoothDampAngle(transform.eulerAngles.y, _playerTargetYaw, ref _playerRotationVelocity, RotationSmoothTime);
            
            if(!_player.IsAiming)
            {
                transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            }
        }       

        _speed = targetSpeed;       

        _controller.Move
            (_targetDirection.normalized * _speed * Time.deltaTime
            + new Vector3(0, _verticalVelocity, 0) * Time.deltaTime);

        // 애니메이션 처리
        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend <= 0.1f) _animationBlend = 0f;

        _animationBlend2 = Vector2.Lerp(_animationBlend2, _input.move.normalized, Time.deltaTime * SpeedChangeRate);
        if (_player.IsAiming)
        {
            _animator.SetFloat(PlayerAnimatorHashes.BlendX, _animationBlend2.x);
            _animator.SetFloat(PlayerAnimatorHashes.BlendY, _animationBlend2.y);
            if (!_animator.GetBool(PlayerAnimatorHashes.Aim)) 
                _animator.SetBool(PlayerAnimatorHashes.Aim, true);
        }
        else
        {
            _animator.SetFloat(PlayerAnimatorHashes.Blend, _animationBlend);
            if (_animator.GetBool(PlayerAnimatorHashes.Aim))
                _animator.SetBool(PlayerAnimatorHashes.Aim, false);
        }

        _animator.SetFloat(PlayerAnimatorHashes.MotionSpeed, inputMagnitude);
    }
    #endregion

    #region 부스트
    /*private void Boost()
    {
        if(_input.boost && !_isBoosting) // 부스팅 중 아니고 키 눌렸다!
        {
            _isBoosting = true;
            _input.boost = false;
        }
        
        if(_isBoosting) // 부스팅 중
        {
            Debug.Log("부스팅 중");
            if (_elapsedBoostTime >= BoostDuration) // 부스팅 시간 끝
            {
                _elapsedBoostTime = 0f;
                _isBoosting = false;
            }
            else
            {
                _elapsedBoostTime += Time.deltaTime;
                float currentSpeed = Mathf.Lerp(BoostSpeed, 0, _elapsedBoostTime / BoostDuration);
                _controller.Move(_targetDirection.normalized * currentSpeed * Time.deltaTime);
            }
        }
    }*/
    #endregion

    private void OnDrawGizmos() // 땅에 있을 땐 초록 / 아니면 빨강
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (IsGround) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(
                transform.position.x,
                transform.position.y - GroundedOffset,
                transform.position.z),
            GroundedRadius);
    }
}
