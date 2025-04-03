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

    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool isZooming;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
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
            zoomInCamera.Priority = 10;
            defaultCamera.Priority = 0;
        }
        else
        {
            isZooming = false;
            playerInput.SwitchCurrentActionMap("Default Player Move");

            //CameraChange
            defaultCamera.Priority = 10;
            zoomInCamera.Priority = 0;
        }
    }

#endif

    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
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
