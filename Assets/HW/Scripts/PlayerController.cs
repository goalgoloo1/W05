using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float DefaultMoveSpeed = 5.0f;

    [Tooltip("Sprint speed of the character in m/s")]
    public float ZoomedInMoveSpeed = 2.335f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Space(10)]
    [Tooltip("The slight height the player lifts during evade (minimal for side movement)")]
    public float EvadeLift = 0.2f; // 최소한의 수직 리프트 (옆으로 가는 느낌 강조)
    [Tooltip("The height the player can evade in zoom mode")]
    public float EvadeHeight = 2.0f; // Evade용 더 높은 점프 높이
    [Tooltip("The distance the player moves forward during evade")]
    public float EvadeDistance = 4.0f; // Evade 수평 이동 거리
    [Tooltip("Time required to pass before being able to evade again")]
    public float EvadeTimeout = 1.0f; // Evade 쿨다운

    //[Header("Cinemachine")]
    //[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    //public GameObject CinemachineCameraTarget;

    //[Tooltip("How far in degrees can you move the camera up")]
    //public float TopClamp = 70.0f;

    //[Tooltip("How far in degrees can you move the camera down")]
    //public float BottomClamp = -30.0f;

    //[Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    //public float CameraAngleOverride = 0.0f;

    //[Tooltip("For locking the camera position on all axis")]
    //public bool LockCameraPosition = false;

    //[SerializeField] private bool isZooming = false;


    [SerializeField] GameObject cameraFollowObject; //cameras will follow this obj that is on player gameobject.
    private CharacterController _controller;
    private CharacterInputs _input;
    private PlayerInput _playerInput;
    private GameObject _mainCamera;
    [SerializeField] private Animator _animator; //it might be able with inspector referencing.

    // player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    private float _evadeTimeRemaining; // Evade 동작 남은 시간
    private Vector3 _evadeDirection; // Evade 방향 저장
    private bool _isEvading; // Evade 중인지 추적

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    private float _evadeTimeoutDelta; // Evade 쿨다운 타이머


    // animation IDs
    //private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDZoom;
    private int _animIDMove;
    private int _animIDFrontMove;
    private int _animIDBackMove;
    private int _animIDSideMove;
    private int _animIDEvade;
    //private int _animIDRightMove;
    //private int _animIDLeftMove;
    //private int _animIDMotionSpeed;

    //Cameras
    private float currentXRotation = 0f;
    private float currentYRotation = 0f;
    public float maxXAngle = 50f;
    //private bool wasZoomingLastFrame = false; // 줌 상태 추적

    private const float _threshold = 0.01f;

    private bool _hasAnimator;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<CharacterInputs>();
        _playerInput = GetComponent<PlayerInput>();
        _mainCamera = Camera.main.gameObject;
        _hasAnimator = TryGetComponent(out _animator); //may be by inspector?

        AssignAnimationIDs();

        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
        _evadeTimeoutDelta = EvadeTimeout; // Evade 쿨다운 초기화
    }

    private void AssignAnimationIDs()
    {
        //_animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDBackMove = Animator.StringToHash("BackMove");
        _animIDEvade = Animator.StringToHash("Evade");
        _animIDSideMove = Animator.StringToHash("SideMove");
        _animIDMove = Animator.StringToHash("Move");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDZoom = Animator.StringToHash("Zoom");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        //_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void Update()
    {
        JumpAndGravity();
        GroundedCheck();
        Move();
    }

    private void Move()
    {
        if(_input.isZooming)
        {
            _animator.SetBool(_animIDZoom, true);

            OnZoomMove();
        }
        else
        {
            _animator.SetBool(_animIDZoom, false);

            DefaultMove();
        }
    }

    private void LateUpdate()
    {
        CameraRotation();
    }



    private void CameraRotation()
    {
        if(_input.isZooming)
        {
            // 플레이어의 초기 Y축 방향으로 카메라 맞춤
            currentYRotation = transform.eulerAngles.y;
            //currentXRotation = 0f; // Tilt 초기화 (필요 시 조정)
            //cameraFollowObject.transform.rotation = Quaternion.Euler(cameraFollowObject.transform.eulerAngles.x, currentYRotation, 0f);


            // Pan/Tilt 입력 적용
            float rotateY = _input.look.x;
            float rotateX = _input.look.y;

            currentYRotation += rotateY;
            currentXRotation -= rotateX;
            currentXRotation = Mathf.Clamp(currentXRotation, -maxXAngle, maxXAngle);

            // cameraFollowObject만 회전, 플레이어 회전과 독립
            cameraFollowObject.transform.rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0f);

            // cameraFollowObject의 월드 회전에서 Y축 값만 가져옴
            float cameraYRotation = cameraFollowObject.transform.eulerAngles.y;

            // 플레이어의 현재 회전에서 Y축만 업데이트
            Vector3 playerRotation = transform.eulerAngles;
            playerRotation.y = cameraYRotation;
            transform.eulerAngles = playerRotation;
        }
        else
        {
            float rotateY = _input.look.x;
            float rotateX = _input.look.y;

            currentYRotation += rotateY;
            currentXRotation -= rotateX;
            currentXRotation = Mathf.Clamp(currentXRotation, -maxXAngle, maxXAngle);

            // cameraFollowObject만 회전, 플레이어 회전과 독립
            cameraFollowObject.transform.rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0f);
        }

    }

    private void OnZoomMove()
    {
        float targetSpeed = ZoomedInMoveSpeed;

        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = _input.move.magnitude;

        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        if (_input.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
        }

        if (!_isEvading) // Evade 중이 아닐 때만 일반 이동
        {
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        // Zoom 상태 애니메이션
        if (_hasAnimator && _input.move != Vector2.zero && !_isEvading)
        {
            Vector3 moveDirection = targetDirection.normalized;
            Vector3 cameraForward = _mainCamera.transform.forward;
            cameraForward.y = 0;
            cameraForward = cameraForward.normalized;

            float angle = Vector3.SignedAngle(cameraForward, moveDirection, Vector3.up);

            if (angle >= -45f && angle <= 45f)
            {
                _animator.SetBool(_animIDMove, true);
                _animator.SetBool(_animIDBackMove, false);
                _animator.SetBool(_animIDSideMove, false);
            }
            else if (angle >= 135f || angle <= -135f)
            {
                _animator.SetBool(_animIDMove, false);
                _animator.SetBool(_animIDBackMove, true);
                _animator.SetBool(_animIDSideMove, false);
            }
            else
            {
                _animator.SetBool(_animIDMove, false);
                _animator.SetBool(_animIDBackMove, false);
                _animator.SetBool(_animIDSideMove, true);
            }
        }
        else if (_hasAnimator && !_isEvading)
        {
            _animator.SetBool(_animIDMove, false);
            _animator.SetBool(_animIDBackMove, false);
            _animator.SetBool(_animIDSideMove, false);
        }
    }
    private void DefaultMove() //Move Logic When Player is not zooming in.
    {
        float targetSpeed = DefaultMoveSpeed;

        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = _input.move.magnitude;

        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        if (_input.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        // Default 상태 애니메이션: 이동 중이면 Move = true
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDMove, _input.move != Vector2.zero);
        }
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, Grounded);
        }
    }

    private void JumpAndGravity()
    {
        if (Grounded)
        {
            _fallTimeoutDelta = FallTimeout;

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }

            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // 줌인 상태에서 Evade 체크
            if (_input.isZooming && _input.evade && _evadeTimeoutDelta <= 0.0f)
            {
                PerformEvade();
            }
            // 기본 점프
            else if (_input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, true);
                }
            }

            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
            if (_evadeTimeoutDelta >= 0.0f)
            {
                _evadeTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = JumpTimeout;
            _evadeTimeoutDelta = EvadeTimeout;

            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else if (_hasAnimator)
            {
                _animator.SetBool(_animIDFreeFall, true);
            }

            _input.jump = false;
            _input.evade = false;
        }

        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;

            if (_isEvading)
            {
                Vector3 evadeDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
                _controller.Move(evadeDirection.normalized * (EvadeDistance * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            }
        }

        // Evade가 끝났는지 체크
        if (_isEvading && Grounded)
        {
            _isEvading = false;
        }
    }

    private void PerformEvade()
    {
        //_verticalVelocity = Mathf.Sqrt(EvadeHeight * -2f * Gravity);
        _isEvading = true;
        _evadeTimeoutDelta = EvadeTimeout;

        if (_hasAnimator)
        {
            Vector3 moveDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            Vector3 cameraForward = _mainCamera.transform.forward;
            cameraForward.y = 0;
            cameraForward = cameraForward.normalized;

            float angle = Vector3.SignedAngle(cameraForward, moveDirection, Vector3.up);

            // 4방향 Evade 트리거 (Trigger로 가정)
            if (angle >= -45f && angle <= 45f) // 앞
            {
                _animator.SetTrigger(_animIDEvade); // "EvadeForward"로 변경 가능
            }
            else if (angle >= 135f || angle <= -135f) // 뒤
            {
                _animator.SetTrigger(_animIDEvade); // "EvadeBackward"로 변경 가능
            }
            else if (angle > 45f && angle < 135f) // 오른쪽
            {
                _animator.SetTrigger(_animIDEvade); // "EvadeRight"로 변경 가능
            }
            else // 왼쪽
            {
                _animator.SetTrigger(_animIDEvade); // "EvadeLeft"로 변경 가능
            }
        }
    }
}
