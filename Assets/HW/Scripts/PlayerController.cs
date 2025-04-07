using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
    [Tooltip("Time required to pass before being able to evade again")]
    public float EvadeTimeout = 5.0f; // Evade ��ٿ�

    [Space(10)]
    [Tooltip("Evade distance in meters")]
    public float EvadeDistance = 2.0f; // ȸ�� �̵� �Ÿ�
    [Tooltip("Evade speed multiplier")]
    public float EvadeSpeedMultiplier = 1.5f; // ȸ�� �ӵ� ��� (�⺻ �ӵ��� ������)


    [SerializeField] GameObject cameraFollowObject; //cameras will follow this obj that is on player gameobject.
    private CharacterController _controller;
    private CharacterInputs _input;
    private PlayerInput _playerInput;
    private GameObject _mainCamera;
    [SerializeField] private Animator _animator; //it might be able with inspector referencing.
    private PlayerManager _playerManager;

    // player
    private float _speed;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    private float _evadeTimeRemaining; // Evade ���� ���� �ð�
    private Vector3 _evadeDirection; // Evade ���� ����
    private bool _isEvading; // Evade ������ ����
    private bool _isMoveDisabled; //������ �� ���� ����.

    private float footStep;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    private float _evadeTimeoutDelta; // Evade ��ٿ� Ÿ�̸�


    // animation IDs
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDZoom;
    private int _animIDMove;
    private int _animIDFrontMove;
    private int _animIDBackMove;
    private int _animIDEvade;
    private int _animIDRightMove;
    private int _animIDLeftMove;
    //private int _animIDMotionSpeed;

    //Cameras
    private float currentXRotation = 0f;
    private float currentYRotation = 0f;
    public float maxXAngle = 50f;
    //private bool wasZoomingLastFrame = false; // �� ���� ����

    private bool _hasAnimator;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<CharacterInputs>();
        _playerInput = GetComponent<PlayerInput>();
        _mainCamera = Camera.main.gameObject;
        _hasAnimator = TryGetComponent(out _animator); //may be by inspector?
        playerManager = PlayerManager.Instance;
        _playerManager = PlayerManager.Instance;

        AssignAnimationIDs();

        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
        _evadeTimeoutDelta = EvadeTimeout; // Evade ��ٿ� �ʱ�ȭ

        _input.onFireAction += Fire;
        _input.onEvadeAction += PerformEvade;
    }

    private void AssignAnimationIDs()
    {
        //_animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDBackMove = Animator.StringToHash("BackMove");
        _animIDEvade = Animator.StringToHash("Evade");
        _animIDRightMove = Animator.StringToHash("RightMove");
        _animIDLeftMove = Animator.StringToHash("LeftMove");
        _animIDFrontMove = Animator.StringToHash("FrontMove");
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

        MakeFootSound();
    }

    private void Move()
    {
        if (_isMoveDisabled) // ������ �� ����
        {
            return;
        }

        if (_isEvading) // ȸ�� ���� ��
        {
            _evadeTimeRemaining -= Time.deltaTime;

            // ȸ�� �� �̵� ó��
            float evadeSpeed = DefaultMoveSpeed * EvadeSpeedMultiplier; // ���� �̵� �ӵ�
            Vector3 evadeMove = _evadeDirection * evadeSpeed * Time.deltaTime;

            // ���� �̵��� ���� (�߷��� JumpAndGravity���� ó��)
            _controller.Move(evadeMove + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            if (_evadeTimeRemaining <= 0.0f && Grounded)
            {
                _isEvading = false; // ȸ�� ����
                Debug.Log("Evade Ended!");
            }
        }
        else // �Ϲ� �̵�
        {
            if (_input.isZooming)
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

        if (_jumpTimeoutDelta >= 0.0f) // ���� ���ð� ����
        {
            _jumpTimeoutDelta -= Time.deltaTime;
        }
        //if (_playerManager.evadeTimeoutDelta >= 0.0f) // ȸ�� ���ð� ����
        //{
        //    _evadeTimeoutDelta -= Time.deltaTime;
        //}
    }

    private void LateUpdate()
    {
        CameraRotation();
    }



    private void CameraRotation()
    {
        if(!_isMoveDisabled)
        {
            if (_input.isZooming)
            {
                // Pan/Tilt �Է� ����
                float rotateY = _input.look.x;
                float rotateX = _input.look.y;

                currentYRotation += rotateY;
                currentXRotation -= rotateX;
                currentXRotation = Mathf.Clamp(currentXRotation, -maxXAngle, maxXAngle);

                // cameraFollowObject�� ȸ��, �÷��̾� ȸ���� ����
                cameraFollowObject.transform.rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0f);

                // cameraFollowObject�� ���� ȸ������ Y�� ���� ������
                float cameraYRotation = cameraFollowObject.transform.eulerAngles.y;

                // �÷��̾��� ���� ȸ������ Y�ุ ������Ʈ
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

                // cameraFollowObject�� ȸ��, �÷��̾� ȸ���� ����
                cameraFollowObject.transform.rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0f);
            }
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

        if (!_isEvading) // Evade ���� �ƴ� ���� �Ϲ� �̵�
        {
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        // Zoom ���� �ִϸ��̼�
        if (_hasAnimator && _input.move != Vector2.zero && !_isEvading)
        {
            Vector3 moveDirection = targetDirection.normalized;
            Vector3 cameraForward = _mainCamera.transform.forward;
            cameraForward.y = 0;
            cameraForward = cameraForward.normalized;

            float angle = Vector3.SignedAngle(cameraForward, moveDirection, Vector3.up);

            // ���⺰ �ִϸ��̼� ����
            if (angle >= -55f && angle <= 55f) // �� (Front)
            {
                _animator.SetBool(_animIDFrontMove, true);
                _animator.SetBool(_animIDBackMove, false);
                _animator.SetBool(_animIDRightMove, false);
                _animator.SetBool(_animIDLeftMove, false);
            }
            else if (angle >= 125f || angle <= -125f) // �� (Back)
            {
                _animator.SetBool(_animIDFrontMove, false);
                _animator.SetBool(_animIDBackMove, true);
                _animator.SetBool(_animIDRightMove, false);
                _animator.SetBool(_animIDLeftMove, false);
            }
            else if (angle > 55f && angle < 125f) // ������ (Right)
            {
                _animator.SetBool(_animIDFrontMove, false);
                _animator.SetBool(_animIDBackMove, false);
                _animator.SetBool(_animIDRightMove, true);
                _animator.SetBool(_animIDLeftMove, false);
            }
            else if (angle < -55f && angle > -125f) // ���� (Left)
            {
                _animator.SetBool(_animIDFrontMove, false);
                _animator.SetBool(_animIDBackMove, false);
                _animator.SetBool(_animIDRightMove, false);
                _animator.SetBool(_animIDLeftMove, true);
            }
        }
        else if (_hasAnimator && !_isEvading)
        {
            // �̵��� ������ ��� ���� �ִϸ��̼� ��
            _animator.SetBool(_animIDFrontMove, false);
            _animator.SetBool(_animIDBackMove, false);
            _animator.SetBool(_animIDRightMove, false);
            _animator.SetBool(_animIDLeftMove, false);
        }
    }
    private void DefaultMove() // Move Logic When Player is not zooming in.
    {
        float targetSpeed = DefaultMoveSpeed;

        if (_input.move == Vector2.zero)
        {
            targetSpeed = 0.0f;
            _animator.SetBool(_animIDMove, false);
        }
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

        if (_input.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

            _animator.SetBool(_animIDMove, true);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        // Default ���� �ִϸ��̼�: ���⺰ Bool �� ����
        if (_hasAnimator && _input.move != Vector2.zero)
        {
            Vector3 moveDirection = targetDirection.normalized;
            Vector3 cameraForward = _mainCamera.transform.forward;
            cameraForward.y = 0;
            cameraForward = cameraForward.normalized;

            float angle = Vector3.SignedAngle(cameraForward, moveDirection, Vector3.up);

            // ���⺰ �ִϸ��̼� ����
            if (angle >= -55f && angle <= 55f) // �� (Front)
            {
                _animator.SetBool(_animIDFrontMove, true);
                _animator.SetBool(_animIDBackMove, false);
                _animator.SetBool(_animIDRightMove, false);
                _animator.SetBool(_animIDLeftMove, false);
            }
            else if (angle >= 135f || angle <= -135f) // �� (Back)
            {
                _animator.SetBool(_animIDFrontMove, false);
                _animator.SetBool(_animIDBackMove, true);
                _animator.SetBool(_animIDRightMove, false);
                _animator.SetBool(_animIDLeftMove, false);
            }
            else if (angle > 55f && angle < 135f) // ������ (Right)
            {
                _animator.SetBool(_animIDFrontMove, false);
                _animator.SetBool(_animIDBackMove, false);
                _animator.SetBool(_animIDRightMove, true);
                _animator.SetBool(_animIDLeftMove, false);
            }
            else if (angle < -55f && angle > -135f) // ���� (Left)
            {
                _animator.SetBool(_animIDFrontMove, false);
                _animator.SetBool(_animIDBackMove, false);
                _animator.SetBool(_animIDRightMove, false);
                _animator.SetBool(_animIDLeftMove, true);
            }
        }
        else if (_hasAnimator)
        {
            // �̵��� ������ ��� ���� �ִϸ��̼� ��
            _animator.SetBool(_animIDFrontMove, false);
            _animator.SetBool(_animIDBackMove, false);
            _animator.SetBool(_animIDRightMove, false);
            _animator.SetBool(_animIDLeftMove, false);
        }
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, Grounded);
        }
    }

    // Gizmos�� ��ü �׸���
    private void OnDrawGizmos()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Gizmos.color = Color.red; // ��ü ���� ����
        Gizmos.DrawWireSphere(spherePosition, GroundedRadius); // ��ü �׸���
    }

    private void JumpAndGravity()
    {
        if (Grounded) // ���� ���� ��
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

            // ���� ��ٿ� ���� (Move()���� ó���ϴ� ���� ����� �̵�)
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }

            if (_input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                //Debug.Log("���� ���� - Velocity: " + _verticalVelocity);
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, true);
                }
            }
        }
        else // ����
        {
            _jumpTimeoutDelta = JumpTimeout;

            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else if (_hasAnimator)
            {
                _animator.SetBool(_animIDFreeFall, true);
            }

            _input.jump = false; // �Է� ����
        }

        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }


    private void PerformEvade()
    {
        if (!_isEvading && _playerManager.evadeTimeoutDelta <= 0.0f && Grounded)
        {
            _animator.SetTrigger(_animIDEvade);
            _isEvading = true;
            _playerManager.evadeTimeoutDelta = _playerManager.evadeTimeout;
            _evadeTimeRemaining = 0.6f;

            Vector3 cameraForward = _mainCamera.transform.forward;
            cameraForward.y = 0;
            cameraForward = cameraForward.normalized;
            Vector3 cameraRight = _mainCamera.transform.right;
            cameraRight.y = 0;
            cameraRight = cameraRight.normalized;

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            AudioManager.Instance.PlayClip(GetComponent<AudioSource>(), "FX_Env_Swish");

            if (inputDirection.magnitude > 0.1f)
            {
                float angle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
                float relativeAngle = angle;

                if (relativeAngle >= -45f && relativeAngle <= 45f)
                    _evadeDirection = cameraForward;
                else if (relativeAngle >= 135f || relativeAngle <= -135f)
                    _evadeDirection = -cameraForward;
                else if (relativeAngle > 45f && relativeAngle < 135f)
                    _evadeDirection = cameraRight;
                else if (relativeAngle < -45f && relativeAngle > -135f)
                    _evadeDirection = -cameraRight;
            }

            _verticalVelocity = Mathf.Sqrt(1f * -2f * Gravity);
            Debug.Log($"Evade Started! Direction: {_evadeDirection}, Vertical Velocity: {_verticalVelocity}");

            StartCoroutine(EnableEvadeRange());
        }
    }

    [SerializeField] GameObject evadeRange;
    public float evadeRangeLastingTime = 0.2f;
    public bool evadeSuccess = false;

    private IEnumerator EnableEvadeRange()
    {
        evadeSuccess = false;
        evadeRange.SetActive(true);
        yield return new WaitForSecondsRealtime(evadeRangeLastingTime);
        evadeRange.SetActive(false);
    }

    [SerializeField] CinemachineCamera enemyTrackCamera;
    [SerializeField] CinemachineTargetGroup targetGroup;
    public float bulletTimeTime = 3f;

    public void OnEvadeSuccess(GameObject enemyBullet)
    {
        GameObject enemyObject = enemyBullet.GetComponent<BulletMovement>().enemyShooter; //get shooter of the bullet.
        playerManager.IncreaseBulletCount(1);
        Instantiate((GameObject)Resources.Load("HW/EvadeSuccessParticle"), transform);
        //ChangeCamera
        //CameraController.Instance.ChangeCamera(enemyTrackCamera);
        //targetGroup.AddMember(enemyObject.transform, 0, 1);

        StartCoroutine(OnEvadeSuccessCoroutine(enemyObject));
    }

    private IEnumerator OnEvadeSuccessCoroutine(GameObject enemyObject)
    {
        Time.timeScale = 0.3f;
        
        _controller.detectCollisions = false;

        yield return new WaitForSecondsRealtime(bulletTimeTime);

        //targetGroup.RemoveMember(enemyObject.transform);
        Time.timeScale = 1f;

        _controller.detectCollisions = true;

        //CameraController.Instance.ReturnCamera(enemyTrackCamera);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (evadeSuccess == false)
            {
                evadeSuccess = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (evadeSuccess == false)
            {
                evadeSuccess = true;
                OnEvadeSuccess(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (evadeSuccess == false)
            {
                evadeSuccess = true;
                OnEvadeSuccess(other.gameObject);
            }
        }
    }

    public void MakeFootSound()
    {
        if (_input.move != Vector2.zero && Grounded && footStep > (_input.isZooming ? 0.525f : 0.3f))
        {
            AudioManager.Instance.PlayClipPrefix(GetComponent<AudioSource>(), "FX_Player_Walking");
            footStep = 0;
        }
        else
        {
            footStep += Time.deltaTime;
        }
    }

    public void SetMoveable(bool value)
    {
        _isMoveDisabled = value;
    }

    [SerializeField] private Transform gunEdgeTransform;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] CinemachineCamera zoomInCamera;
    public void Fire()
    {
        if (playerManager.fireCoolDownDelta <= 0f && !_isMoveDisabled && playerManager.remainingBullet > 0)
        {
            CinemachineThirdPersonAim cinemachineAim = zoomInCamera.GetComponent<CinemachineThirdPersonAim>();

            Vector3 aimingTarget = cinemachineAim.AimTarget;

            GameObject playerBullet = Instantiate(bulletPrefab, gunEdgeTransform.position, Quaternion.identity);
            playerBullet.GetComponent<PlayerBulletMovement>().SetTargetPosition(aimingTarget);

            playerManager.Shot();
            AudioManager.Instance.PlayClipPrefix(GetComponent<AudioSource>(), "FX_Player_Shot");
        }

    }

    public void OnDeath()
    {
        Debug.Log("player Death");
        if (!_isMoveDisabled) AudioManager.Instance.PlayClipPrefix(GetComponent<AudioSource>(), "FX_Player_Death");

        Instantiate((GameObject)Resources.Load("HW/PlayerDeathParticle"), transform.position, Quaternion.identity);
        _isMoveDisabled = true;

        MenuUIManager.Instance.ShowMenuUIGameover();
    }
}
