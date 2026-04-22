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
            if (IsGrounded())
            {
                stateMachine.ChangeState(new GroundedState(stateMachine));
                return;
            }

            if (!IsWalled())
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

        private bool IsGrounded()
        {
            return Physics2D.OverlapCircle(stateMachine.transform.position, stats.groundCheckRadius, stats.groundLayer);
        }

        private bool IsWalled()
        {
            // Detecta pared en la dirección en la que mira el jugador
            Vector2 checkPos = stateMachine.transform.position + new Vector3(stateMachine.transform.localScale.x * 0.5f, 0, 0);
            return Physics2D.OverlapCircle(checkPos, stats.wallCheckRadius, stats.groundLayer);
        }
    }
}
