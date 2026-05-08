using UnityEngine;

namespace DeathCloud.Player.States
{
    using Core;

    public class WallState : PlayerState
    {
        private float _horizontalInput;

        public WallState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            input.JumpEvent += OnWallJump;
            input.MoveEvent += OnMove;
            input.DashEvent += OnDash;

            _horizontalInput = input.MoveValue.x;
        }

        public override void Exit()
        {
            input.JumpEvent -= OnWallJump;
            input.MoveEvent -= OnMove;
            input.DashEvent -= OnDash;
        }

        private void OnMove(Vector2 move)
        {
            _horizontalInput = move.x;
        }

        private void OnDash()
        {
            stateMachine.ChangeState(new DashState(stateMachine));
        }

        private void OnWallJump()
        {
            float direction = -stateMachine.transform.localScale.x;
            stateMachine.RB.linearVelocity = new Vector2(direction * stats.wallJumpPower.x, stats.wallJumpPower.y);
            stateMachine.ChangeState(new AirborneState(stateMachine, true));
        }

        public override void Update()
        {
            if (stateMachine.IsGrounded())
            {
                stateMachine.ChangeState(new GroundedState(stateMachine));
                return;
            }

            // Si ya no tocamos la pared o dejamos de presionar la dirección de la pared
            if (!stateMachine.IsWalled(stateMachine.transform.localScale.x))
            {
                stateMachine.ChangeState(new AirborneState(stateMachine));
            }
        }

        public override void FixedUpdate()
        {
            // Deslizamiento lento en la pared
            float slidingVel = Mathf.Clamp(stateMachine.RB.linearVelocity.y, -stats.wallSlidingSpeed, float.MaxValue);
            stateMachine.RB.linearVelocity = new Vector2(stateMachine.RB.linearVelocity.x, slidingVel);
        }
    }
}
