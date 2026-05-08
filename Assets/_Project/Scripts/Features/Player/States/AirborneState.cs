using UnityEngine;

namespace DeathCloud.Player.States
{
    using Core;

    public class AirborneState : PlayerState
    {
        private float _horizontalInput;
        private float _coyoteTimer;
        private bool _hasJumped;

        public AirborneState(PlayerStateMachine stateMachine, bool isJumping = false) : base(stateMachine)
        {
            _hasJumped = isJumping;
            _coyoteTimer = isJumping ? 0 : stats.coyoteTime;
        }

        public override void Enter()
        {
            input.MoveEvent += OnMove;
            input.GrappleEvent += OnTryGrapple;
            input.JumpEvent += OnJumpInput;
            input.DashEvent += OnDashInput;
        }

        public override void Exit()
        {
            input.MoveEvent -= OnMove;
            input.GrappleEvent -= OnTryGrapple;
            input.JumpEvent -= OnJumpInput;
            input.DashEvent -= OnDashInput;
        }

        private void OnMove(Vector2 move)
        {
            _horizontalInput = move.x;
        }

        private void OnJumpInput()
        {
            if (!_hasJumped && _coyoteTimer > 0)
            {
                // Salto Coyote
                stateMachine.RB.linearVelocity = new Vector2(stateMachine.RB.linearVelocity.x, stats.jumpForce);
                _hasJumped = true;
                _coyoteTimer = 0;
            }
            else
            {
                // Jump Buffer: Le notificamos a la StateMachine que el jugador quiere saltar al aterrizar
                stateMachine.SetJumpBuffer();
            }
        }

        private void OnDashInput()
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

        public override void Update()
        {
            if (_coyoteTimer > 0) _coyoteTimer -= Time.deltaTime;

            if (IsGrounded() && stateMachine.RB.linearVelocity.y <= 0.1f)
            {
                stateMachine.ChangeState(new GroundedState(stateMachine));
                return;
            }

            if (IsWalled() && _horizontalInput != 0)
            {
                stateMachine.ChangeState(new WallState(stateMachine));
            }
            
            HandleFlip();
        }

        private bool IsWalled()
        {
            Vector2 checkPos = stateMachine.transform.position + new Vector3(stateMachine.transform.localScale.x * 0.5f, 0, 0);
            return Physics2D.OverlapCircle(checkPos, stats.wallCheckRadius, stats.groundLayer);
        }

        public override void FixedUpdate()
        {
            // Movimiento lateral en aire (aquí podrías aplicar una inercia diferente si quisieras)
            stateMachine.RB.linearVelocity = new Vector2(_horizontalInput * stats.moveSpeed, stateMachine.RB.linearVelocity.y);
        }

        private bool IsGrounded()
        {
            return Physics2D.OverlapCircle(stateMachine.transform.position, stats.groundCheckRadius, stats.groundLayer);
        }

        private void HandleFlip()
        {
            Vector3 currentScale = stateMachine.transform.localScale;
            if (_horizontalInput > 0)
            {
                currentScale.x = Mathf.Abs(currentScale.x);
            }
            else if (_horizontalInput < 0)
            {
                currentScale.x = -Mathf.Abs(currentScale.x);
            }
            stateMachine.transform.localScale = currentScale;
        }
    }
}
