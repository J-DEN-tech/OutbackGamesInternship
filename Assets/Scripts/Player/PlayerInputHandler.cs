using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput playerInput;
    private Camera cam;

    public Vector2 RawMovementInput { get; private set; }
    public Vector2 RawDashDirectionInput { get; private set; }
    public Vector2Int DashDirectionInput { get; private set; }
    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }
    public bool JumpInput { get; private set; }
    public bool JumpInputStop { get; private set; }
    public bool DashInput { get; private set; }
    public bool DashInputStop { get; private set; }

    [Header("Input Feedback Events")]
    [SerializeField] private UnityEvent OnMove = new UnityEvent();
    [SerializeField] private UnityEvent OnJump = new UnityEvent();
    [SerializeField] private UnityEvent OnJumpStop = new UnityEvent();
    [SerializeField] private UnityEvent OnDash = new UnityEvent();
    [SerializeField] private UnityEvent OnDashStop = new UnityEvent();
    [SerializeField] private UnityEvent OnDashDirection = new UnityEvent();

    private List<PlayerActionItem> inputBuffer = new List<PlayerActionItem>();
    [HideInInspector] public bool isActionAllowed;
    private InputAction.CallbackContext moveContext;
    private InputAction.CallbackContext jumpContext;
    private InputAction.CallbackContext dashContext;
    private InputAction.CallbackContext dashDirectionContext;
    private InputAction.CallbackContext nullContext;

    [Space]
    [SerializeField]
    private float inputHoldTime = 0.2f;

    private float jumpInputStartTime;
    private float dashInputStartTime;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        cam = Camera.main;
    }

    private void Update()
    {
        CheckJumpInputHoldTime();
        CheckDashInputHoldTime();

        if (isActionAllowed)
        {
            TryBufferedAction();
        }
    }

    private void TryBufferedAction()
    {
        if(inputBuffer.Count > 0)
        {
            foreach(PlayerActionItem playerAction in inputBuffer.ToArray())
            {
                inputBuffer.Remove(playerAction);
                if (playerAction.CheckIfValid())
                {
                    if(playerAction.action == PlayerActionItem.InputAction.Move)
                    {
                        DoMove(moveContext);
                    }
                    else if (playerAction.action == PlayerActionItem.InputAction.Jump)
                    {
                        DoJump(jumpContext);
                    }
                    else if(playerAction.action == PlayerActionItem.InputAction.Dash)
                    {
                        DoDash(dashContext);
                    }
                    else if(playerAction.action == PlayerActionItem.InputAction.DashDirection)
                    {
                        DoDashDirection(dashDirectionContext);
                    }
                    break;
                }
            }
        }
    }

    #region Move Functions
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveContext = context;
            inputBuffer.Add(new PlayerActionItem(PlayerActionItem.InputAction.Move, Time.time));
        }

        if (context.canceled)
        {
            DoMove(nullContext);
        }
    }

    public void DoMove(InputAction.CallbackContext context)
    {
        RawMovementInput = context.ReadValue<Vector2>();

        OnMove.Invoke();

        NormInputX = Mathf.RoundToInt(RawMovementInput.x);
        NormInputY = Mathf.RoundToInt(RawMovementInput.y);

        isActionAllowed = false;
    }
    #endregion

    #region Jump Functions
    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpContext = context;
            inputBuffer.Add(new PlayerActionItem(PlayerActionItem.InputAction.Jump, Time.time));
        }

        if (context.canceled)
        {
            DoJump(nullContext);
        }
    }

    public void DoJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnJump.Invoke();

            JumpInput = true;
            JumpInputStop = false;
            jumpInputStartTime = Time.time;
        }

        if (context.canceled)
        {
            OnJumpStop.Invoke();

            JumpInputStop = true;
        }
    }
    #endregion

    #region Dash FunctionsS
    public void OnDashInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            dashContext = context;
            inputBuffer.Add(new PlayerActionItem(PlayerActionItem.InputAction.Dash, Time.time));
        }

        if (context.canceled)
        {
            DoDash(nullContext);
        }
    }

    public void DoDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnDash.Invoke();

            DashInput = true;
            DashInputStop = false;
            dashInputStartTime = Time.time;
        }
        else if (context.canceled)
        {
            OnDashStop.Invoke();

            DashInputStop = true;
        }
    }
    #endregion

    #region Dash Direction Functions
    public void OnDashDirectionInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            dashDirectionContext = context;
            inputBuffer.Add(new PlayerActionItem(PlayerActionItem.InputAction.DashDirection, Time.time));
        }

        if (context.canceled)
        {
            DoDashDirection(nullContext);
        }
    }

    public void DoDashDirection(InputAction.CallbackContext context)
    {
        RawDashDirectionInput = context.ReadValue<Vector2>();

        OnDashDirection.Invoke();

        if (playerInput.currentControlScheme == "KeyboardMouse")
        {
            RawDashDirectionInput = cam.ScreenToWorldPoint((Vector3)RawDashDirectionInput) - transform.position;
        }

        DashDirectionInput = Vector2Int.RoundToInt(RawDashDirectionInput.normalized);
    }
    #endregion

    public void UseJumpInput() => JumpInput = false;

    public void UseDashInput() => DashInput = false;

    private void CheckJumpInputHoldTime()
    {
        if (Time.time >= jumpInputStartTime + inputHoldTime)
        {
            JumpInput = false;
        }
    }

    private void CheckDashInputHoldTime()
    {
        if (Time.time >= dashInputStartTime + inputHoldTime)
        {
            DashInput = false;
        }
    }
}