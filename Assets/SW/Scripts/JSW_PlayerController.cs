using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class JSW_PlayerController : MonoBehaviour
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

    public bool isStop;

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

    [SerializeField] private bool isZooming = false;

    [SerializeField] List<CinemachineCamera> playerCameras; //default camera, zoom in camera.
    [SerializeField] GameObject cameraFollowObject; //cameras will follow this obj that is on player gameobject.
    private CharacterController _controller;
    private CharacterInputs _input;
    private PlayerInput _playerInput;
    private GameObject _mainCamera;

    // player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<CharacterInputs>();
        _playerInput = GetComponent<PlayerInput>();
        _mainCamera = Camera.main.gameObject;
    }

    private void Update()
    {
        JumpAndGravity();
        //GroundedCheck();
        if (!isStop) Move();
    }

    private void Move()
    {
        if (isZooming)
        {
            OnZoomMove();
        }
        else
        {
            DefaultMove();
        }
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {

    }

    private void OnZoomMove() //Move Logic When Player Is Zooming in.
    {
        float targetSpeed = DefaultMoveSpeed;
        if (_input.move == Vector2.zero) targetSpeed = 0.0f;
    }

    private void DefaultMove() //Move Logic When Player is not zooming in.
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        //float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
        float targetSpeed = ZoomedInMoveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        //float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;
        float inputMagnitude = _input.move.magnitude;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // normalise input direction
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (_input.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }


        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // move the player
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                         new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        //// update animator if using character
        //if (_hasAnimator)
        //{
        //    _animator.SetFloat(_animIDSpeed, _animationBlend);
        //    _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        //}
    }

    private void JumpAndGravity()
    {
        if (Grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;

            //// update animator if using character
            //if (_hasAnimator)
            //{
            //    _animator.SetBool(_animIDJump, false);
            //    _animator.SetBool(_animIDFreeFall, false);
            //}

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (_input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                //// update animator if using character
                //if (_hasAnimator)
                //{
                //    _animator.SetBool(_animIDJump, true);
                //}
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                //// update animator if using character
                //if (_hasAnimator)
                //{
                //    _animator.SetBool(_animIDFreeFall, true);
                //}
            }

            // if we are not grounded, do not jump
            _input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }
}
