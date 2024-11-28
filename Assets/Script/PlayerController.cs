using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    //=====Player Move Values=====
    [field: SerializeField] public float WalkSpeed { get; private set; } = 5.0f;
    [field: SerializeField] public float RunSpeed { get; private set; } = 10.0f;
    [field: SerializeField] public float Gravity { get; set; } = -9.81f;
    [field: SerializeField] public float JumpHeight { get; private set; } = 1.2f;
    [field: SerializeField] public float RotationSmoothTime { get; private set; } = 0.1f;

    //=====Ground and Air=====
    [field: SerializeField] public bool IsGround { get; private set; } = false;
    [field: SerializeField] public float GroundedRadius { get; private set; }
    private float GroundedOffset = 0.05f;
    [field: SerializeField] public LayerMask GroundLayers;

    //=====Player Move Timeout=====
    [field: SerializeField] public float JumpTimeout { get; private set; } = 0.3f;
    [field: SerializeField] public float FallTimeout { get; private set; } = 0.1f;
    private float JumpTimeoutDelta;
    private float FallTimeoutDelta;

    //player move value
    private Vector3 _inputDirection;
    private float _verticalVelocity = 0f;
    private float _playerTargetYaw;
    private float _speed; // 현재 속도
    private float _playerRotationVelocity;


    //=====Camera=====
    [field: SerializeField] public Transform CameraRoot { get; private set; }
    [field: SerializeField] public float TopClamp { get; private set; } = 70.0f;
    [field: SerializeField] public float BottomClamp { get; private set; } = -30.0f;
    [field: SerializeField] public float Sensitivity { get; private set; } = 2f;
    private float _cameraTargetYaw;    // 횡 이동
    private float _cameraTargetPitch;  // 종 이동

    //References
    private GameObject _mainCamera;
    private CharacterController _controller;
    private PlayerInputs _input;

    private void Awake()
    {
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputs>();
    }

    void Start()
    {
        _cameraTargetYaw = CameraRoot.rotation.eulerAngles.y;        
        GroundedRadius = _controller.radius;    
    }

    void Update()
    {
        GroundCheck();
        JumpAndGravity();
        CapturePlayerDirection();
        Move();
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

    private void JumpAndGravity()
    {
        if(IsGround)
        {
            FallTimeoutDelta = FallTimeout;

            if (IsGround && _verticalVelocity <= 0.0f)
                _verticalVelocity = -2f;

            if (_input.jump && JumpTimeoutDelta <= 0f)
            {
                _verticalVelocity = Mathf.Sqrt(JumpHeight * 2 * -Gravity);
            }

            if(JumpTimeoutDelta >= 0.0f)
                JumpTimeoutDelta -= Time.deltaTime;
        }
        else
        {
            JumpTimeoutDelta = JumpTimeout;
            _input.jump = false;
        }
        

        
        _verticalVelocity += Gravity * Time.deltaTime;
    }

    private void GroundCheck() 
    {
        // 밑에 캐릭터 컨트롤러랑 지름 똑같은 구체 만들어서 살짝 내리고 닿는지 체크
        Vector3 spherePosition = 
            new Vector3(
                transform.position.x, 
                transform.position.y - (_controller.height / 2) + GroundedRadius - GroundedOffset,
                transform.position.z);
        IsGround = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);
    }

    private void CapturePlayerDirection()
    {
        _inputDirection = new Vector3(_input.move.x, 0, _input.move.y).normalized;
        if (_input.move != Vector2.zero)
        {
            // 메인 카메라 yaw 값 + 입력의 yaw 값 -> Atan2 * Rad머시기 그거
            _playerTargetYaw = _mainCamera.transform.eulerAngles.y + (Mathf.Atan2(_inputDirection.x, _inputDirection.z) * Mathf.Rad2Deg);
        }
    }

    private void Move()
    {
        // 목표 속도
        float targetSpeed = WalkSpeed;
        if (_input.move == Vector2.zero)
        {
            targetSpeed = 0;
        }
        else 
        {
            //회전
            float yaw = Mathf.SmoothDampAngle(transform.eulerAngles.y, _playerTargetYaw, ref _playerRotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        }
        _speed = targetSpeed;

        // 오일러 * forward -> 원하는 벡터. 아무튼 됨
        Vector3 targetDirection = Quaternion.Euler(0f, _playerTargetYaw, 0f) * Vector3.forward; 

        _controller.Move
            (targetDirection.normalized * _speed * Time.deltaTime
            + new Vector3(0, _verticalVelocity, 0) * Time.deltaTime);
    }


    
    /*private void OnDrawGizmos() // 땅에 있을 땐 초록 / 아니면 빨강
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (IsGround) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(
                transform.position.x,
                transform.position.y - (_controller.height / 2) + GroundedRadius - GroundedOffset,
                transform.position.z),
            GroundedRadius);
    }*/
}
