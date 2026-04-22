using UnityEngine;

namespace DeathCloud.Player.States
{
    using Core;

    public class GroundedState : PlayerState
    {
        private float _horizontalInput;

        public GroundedState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            input.JumpEvent += OnJump;
            input.MoveEvent += OnMove;
            input.GrappleEvent += OnTryGrapple;
            input.DashEvent += OnDash;

            // Jump Buffer logic: si ya veníamos con ganas de saltar, saltamos ahora
            if (stateMachine.HasJumpBuffer)
            {
                stateMachine.ConsumeJumpBuffer();
                OnJump();
            }
        }

        public override void Exit()
        {
            input.JumpEvent -= OnJump;
            input.MoveEvent -= OnMove;
            input.GrappleEvent -= OnTryGrapple;
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

        private void OnTryGrapple()
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - (Vector2)stateMachine.transform.position).normalized;
            
            RaycastHit2D hit = Physics2D.Raycast(stateMachine.transform.position, direction, stats.maxHookDistance, stats.hookableLayer);
            
            if (hit.collider != null)
            {
                stateMachine.ChangeState(new HookedState(stateMachine, hit.point));
            }
        }

        private void OnJump()
        {
            stateMachine.RB.linearVelocity = new Vector2(stateMachine.RB.linearVelocity.x, stats.jumpForce);
            stateMachine.ChangeState(new AirborneState(stateMachine));
        }

        public override void Update()
        {
            // Verificación de caída
            if (!IsGrounded())
            {
                stateMachine.ChangeState(new AirborneState(stateMachine));
            }

            HandleFlip();
        }

        public override void FixedUpdate()
        {
            stateMachine.RB.linearVelocity = new Vector2(_horizontalInput * stats.moveSpeed, stateMachine.RB.linearVelocity.y);
        }

        private bool IsGrounded()
        {
            // Implementación simplificada del sensor que ya tenía el usuario
            return Physics2D.OverlapCircle(stateMachine.transform.position, stats.groundCheckRadius, stats.groundLayer);
        }

        private void HandleFlip()
        {
            if (_horizontalInput > 0) stateMachine.transform.localScale = new Vector3(1, 1, 1);
            else if (_horizontalInput < 0) stateMachine.transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
