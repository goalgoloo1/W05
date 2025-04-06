using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handle Input.
/// </summary>
public class CharacterInputs : MonoBehaviour
{
    [Header("Get Component")]
    [SerializeField] PlayerInput playerInput;
    [SerializeField] CinemachineCamera defaultCamera;
    [SerializeField] CinemachineCamera zoomInCamera;

    public Action onFireAction;
    public Action onEvadeAction;

    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool evade; // Evade 입력 추가
    public bool isZooming;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }


    [SerializeField] private float smoothSpeed = 0.1f;
    private Vector2 rawLook; // 원시 입력값 임시 저장

    void Update()
    {
        //look = Vector2.Lerp(look, rawLook, smoothSpeed * Time.deltaTime);
        look = rawLook;
    }


#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }


    public void OnZoom()
    {
        if(isZooming == false) //default state.
        {
            isZooming = true;
            playerInput.SwitchCurrentActionMap("Zoomed In Player Move");

            //CameraChange
            CameraController.Instance.ChangeCamera(zoomInCamera);
        }
        else
        {
            isZooming = false;
            playerInput.SwitchCurrentActionMap("Default Player Move");

            //CameraChange
            CameraController.Instance.ChangeCamera(defaultCamera);
        }
    }

    public void OnJump(InputValue value) //Default Action map -> Jump.
    {
        JumpInput(value.isPressed);
    }

    public void OnEvade(InputValue value) //Zoomed In Action Map -> Evade.
    {
        if (isZooming) // 줌인 상태에서만 Evade 작동
        {
            onEvadeAction?.Invoke();
        }
        else
        {
            JumpInput(value.isPressed); // 줌아웃 상태에서는 기본 점프
        }
    }

    public void OnFire(InputValue value)
    {
        onFireAction?.Invoke();
    }

#endif

    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        rawLook = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        jump = newJumpState;
    }

    public void EvadeInput(bool newEvadeState)
    {
        evade = newEvadeState;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
