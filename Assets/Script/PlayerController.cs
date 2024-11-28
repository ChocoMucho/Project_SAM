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
    private float _speed; // ���� �ӵ�
    private float _playerRotationVelocity;


    //=====Camera=====
    [field: SerializeField] public Transform CameraRoot { get; private set; }
    [field: SerializeField] public float TopClamp { get; private set; } = 70.0f;
    [field: SerializeField] public float BottomClamp { get; private set; } = -30.0f;
    [field: SerializeField] public float Sensitivity { get; private set; } = 2f;
    private float _cameraTargetYaw;    // Ⱦ �̵�
    private float _cameraTargetPitch;  // �� �̵�

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

    #region ī�޶�
    private void CameraRotation()
    {
        // �Է� ���� ���, ��ġ ���� ����
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
        // -360~360 ����
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
        // �ؿ� ĳ���� ��Ʈ�ѷ��� ���� �Ȱ��� ��ü ���� ��¦ ������ ����� üũ
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
            // ���� ī�޶� yaw �� + �Է��� yaw �� -> Atan2 * Rad�ӽñ� �װ�
            _playerTargetYaw = _mainCamera.transform.eulerAngles.y + (Mathf.Atan2(_inputDirection.x, _inputDirection.z) * Mathf.Rad2Deg);
        }
    }

    private void Move()
    {
        // ��ǥ �ӵ�
        float targetSpeed = WalkSpeed;
        if (_input.move == Vector2.zero)
        {
            targetSpeed = 0;
        }
        else 
        {
            //ȸ��
            float yaw = Mathf.SmoothDampAngle(transform.eulerAngles.y, _playerTargetYaw, ref _playerRotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        }
        _speed = targetSpeed;

        // ���Ϸ� * forward -> ���ϴ� ����. �ƹ�ư ��
        Vector3 targetDirection = Quaternion.Euler(0f, _playerTargetYaw, 0f) * Vector3.forward; 

        _controller.Move
            (targetDirection.normalized * _speed * Time.deltaTime
            + new Vector3(0, _verticalVelocity, 0) * Time.deltaTime);
    }


    
    /*private void OnDrawGizmos() // ���� ���� �� �ʷ� / �ƴϸ� ����
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
