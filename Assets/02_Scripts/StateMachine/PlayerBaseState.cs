using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBaseState : IState
{
    protected PlayerStateMachine stateMachine;
    protected readonly PlayerGroundData groundData;

    public PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        groundData = stateMachine.Player.Data.GroundData;
    }

    public virtual void Enter()
    {
        AddInputActionsCallbacks();
    }

    public virtual void Exit()
    {
        RemoveInputActionsCallbacks();
    }

    protected virtual void AddInputActionsCallbacks()
    {
        PlayerController input = stateMachine.Player.Input;
        input.playerActions.Movement.performed += OnMovementInput;
        input.playerActions.Movement.canceled += OnMovementInput;
        input.playerActions.Run.started += OnRunStarted;    
    }

    protected virtual void RemoveInputActionsCallbacks()
    {
        PlayerController input = stateMachine.Player.Input;
        input.playerActions.Movement.performed -= OnMovementInput;
        input.playerActions.Movement.canceled -= OnMovementInput;
        input.playerActions.Run.started -= OnRunStarted;
    }

    protected virtual void OnRunStarted(InputAction.CallbackContext context)
    {
    }

    protected virtual void OnMovementInput(InputAction.CallbackContext context)
    {
        // 이동 입력을 처리
        stateMachine.MovementInput = context.ReadValue<Vector2>();
    }

    public virtual void HandleInput()
    {
        // 입력을 읽기만 함
        ReadMovementInput();
    }

    public virtual void PhysicsUpdate()
    {
    }

    public virtual void Update()
    {
        Move();
    }

    protected void StartAnimation(int animationHash)
    {
        stateMachine.Player.Animator.SetBool(animationHash, true);
    }

    protected void StopAnimation(int animationHash)
    {
        stateMachine.Player.Animator.SetBool(animationHash, false);
    }
    
    private void ReadMovementInput()
    {
        // 이동 입력을 읽기만 함
        stateMachine.MovementInput = stateMachine.Player.Input.playerActions.Movement.ReadValue<Vector2>();
    }
    
    private void Move()
    {
        Vector3 movementDirection = GetMovementDirection();
        // 회전은 마우스 입력에 의해 처리됨
        Move(movementDirection);
    }
    
    private Vector3 GetMovementDirection()
    {
        Vector3 forward = stateMachine.MainCamTransform.forward;
        Vector3 right = stateMachine.MainCamTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        return forward * stateMachine.MovementInput.y + right * stateMachine.MovementInput.x; 
    }
    
    private void Move(Vector3 direction)
    {
        float movementSpeed = GetMovementSpeed();
        stateMachine.Player.Controller.Move((direction * movementSpeed) * Time.deltaTime);
    }

    private float GetMovementSpeed()
    {
        float movementSpeed = stateMachine.MovementSpeed * stateMachine.MovementSpeedModifier;
        return movementSpeed;
    }
}
