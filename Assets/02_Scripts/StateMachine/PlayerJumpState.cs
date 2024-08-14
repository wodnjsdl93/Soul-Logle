public class PlayerJumpState : PlayerAirState
{
    public PlayerJumpState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
  
    }

    public override void Enter()
    {
        stateMachine.JumpForce = stateMachine.Player.Data.AirData.JumpForce;
        stateMachine.Player.ForceReceiver.Jump(stateMachine.JumpForce);

        base.Enter();

        StartAnimation(stateMachine.Player.AnimationData.JumpParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(stateMachine.Player.AnimationData.JumpParameterHash);
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.Player.Controller.velocity.y <=0)
        {
            stateMachine.ChangeState(stateMachine.FallState);
            return;
        }
    }

}
