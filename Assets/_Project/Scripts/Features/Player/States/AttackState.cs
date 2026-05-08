using UnityEngine;

namespace DeathCloud.Player.States
{
    using Core;

    public class AttackState : PlayerState 
    {
        private float attackDuration = 0.4f; // Duración temporal hasta tener animaciones
        private float timePassed;

        public AttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            Debug.Log("[AttackState] Atacando...");
            timePassed = 0f;
            
            // Frenar al jugador horizontalmente al atacar (mantiene la velocidad Y por si cae)
            stateMachine.RB.linearVelocity = new Vector2(0, stateMachine.RB.linearVelocity.y); 
        }

        public override void Update()
        {
            timePassed += Time.deltaTime;
            CheckSwitchStates();
        }

        public override void Exit()
        {
            Debug.Log("[AttackState] Fin del ataque.");
        }

        private void CheckSwitchStates()
        {
            if (timePassed >= attackDuration)
            {
                if (IsGrounded())
                {
                    stateMachine.ChangeState(new GroundedState(stateMachine));
                }
                else
                {
                    stateMachine.ChangeState(new AirborneState(stateMachine));
                }
            }
        }

        private bool IsGrounded()
        {
            return Physics2D.OverlapCircle(stateMachine.transform.position, stats.groundCheckRadius, stats.groundLayer);
        }
    }
}
