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
            input.AttackEvent += OnAttackPressed;

            _horizontalInput = input.MoveValue.x;

            // Minecraft-style Jump y Jump Buffer: 
            // Si el jugador mantiene el salto o presionó justo antes de caer, saltamos al instante.
            if (input.IsJumpHeld || stateMachine.HasJumpBuffer)
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
            input.AttackEvent -= OnAttackPressed;
        }

        private void OnMove(Vector2 move)
        {
            _horizontalInput = move.x;
        }

        private void OnDash()
        {
            stateMachine.ChangeState(new DashState(stateMachine));
        }

        private void OnAttackPressed()
        {
            stateMachine.ChangeState(new AttackState(stateMachine));
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
            // Verificación de caída usando el sistema centralizado
            if (!stateMachine.IsGrounded())
            {
                stateMachine.ChangeState(new AirborneState(stateMachine));
                return;
            }

            // Minecraft-style Jump: Salto continuo si se mantiene presionada la tecla
            if (input.IsJumpHeld)
            {
                OnJump();
                return;
            }

            HandleFlip();
        }

        public override void FixedUpdate()
        {
            stateMachine.RB.linearVelocity = new Vector2(_horizontalInput * stats.moveSpeed, stateMachine.RB.linearVelocity.y);
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
