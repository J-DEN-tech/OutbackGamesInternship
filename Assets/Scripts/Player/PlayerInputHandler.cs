using System;
using System.Collections;
using System.Collections.Generic;
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
    //public bool GrabInput { get; private set; }
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
    private bool doMove;
    private bool doJump;
    private bool doDash;
    private bool doDashDirection;

    //public bool[] AttackInputs { get; private set; }
    [Space]
    [SerializeField]
    private float inputHoldTime = 0.2f;

    private float jumpInputStartTime;
    private float dashInputStartTime;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        //int count = Enum.GetValues(typeof(CombatInputs)).Length;
        //AttackInputs = new bool[count];

        cam = Camera.main;
    }

    private void Update()
    {
        CheckJumpInputHoldTime();
        CheckDashInputHoldTime();

        CheckInput();
        if (isActionAllowed)
        {
            TryBufferedAction();
        }
    }

    private void CheckInput()
    {
        if (playerInput.actions["Move"].WasPerformedThisFrame())
        {
            inputBuffer.Add(new PlayerActionItem(PlayerActionItem.InputAction.Move, Time.time));
        }
        else if (playerInput.actions["Jump"].WasPerformedThisFrame())
        {
            inputBuffer.Add(new PlayerActionItem(PlayerActionItem.InputAction.Jump, Time.time));
        }
        else if (playerInput.actions["Dash"].WasPerformedThisFrame())
        {
            inputBuffer.Add(new PlayerActionItem(PlayerActionItem.InputAction.Dash, Time.time));
        }
        else if (playerInput.actions["DashDirection"].WasPerformedThisFrame())
        {
            inputBuffer.Add(new PlayerActionItem(PlayerActionItem.InputAction.DashDirection, Time.time));
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
                        doMove = true;
                    }
                    else if (playerAction.action == PlayerActionItem.InputAction.Jump)
                    {
                        doJump = true;
                    }
                    else if(playerAction.action == PlayerActionItem.InputAction.Dash)
                    {
                        doDash = true;
                    }
                    else if(playerAction.action == PlayerActionItem.InputAction.DashDirection)
                    {
                        doDashDirection = true;
                    }
                    break;
                }
            }
        }
    }

    /*public void OnPrimaryAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            AttackInputs[(int)CombatInputs.primary] = true;
        }

        if (context.canceled)
        {
            AttackInputs[(int)CombatInputs.primary] = false;
        }
    }

    public void OnSecondaryAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            AttackInputs[(int)CombatInputs.secondary] = true;
        }

        if (context.canceled)
        {
            AttackInputs[(int)CombatInputs.secondary] = false;
        }
    }*/

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (doMove)
        {
            RawMovementInput = context.ReadValue<Vector2>();

            OnMove.Invoke();

            NormInputX = Mathf.RoundToInt(RawMovementInput.x);
            NormInputY = Mathf.RoundToInt(RawMovementInput.y);

            doMove = false;
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (doJump)
        {
            if (context.started)
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
        doJump = false;
    }

    /*public void OnGrabInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GrabInput = true;
        }

        if (context.canceled)
        {
            GrabInput = false;
        }
    }*/

    public void OnDashInput(InputAction.CallbackContext context)
    {
        if (doDash)
        {
            if (context.started)
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
        doDash = false;
    }

    public void OnDashDirectionInput(InputAction.CallbackContext context)
    {
        if (doDashDirection)
        {
            RawDashDirectionInput = context.ReadValue<Vector2>();

            OnDashDirection.Invoke();

            if (playerInput.currentControlScheme == "KeyboardMouse")
            {
                RawDashDirectionInput = cam.ScreenToWorldPoint((Vector3)RawDashDirectionInput) - transform.position;
            }

            DashDirectionInput = Vector2Int.RoundToInt(RawDashDirectionInput.normalized);
        }
        doDashDirection = false;
    }

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

/*public enum CombatInputs
{
    primary,
    secondary
}*/
