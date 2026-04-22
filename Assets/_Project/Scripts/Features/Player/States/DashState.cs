using UnityEngine;

namespace DeathCloud.Player.States
{
    using Core;

    public class DashState : PlayerState
    {
        private float _timer;
        private Vector2 _dashDirection;
        private float _originalGravity;

        public DashState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            _timer = stats.dashDuration;
            
            // Dirección del Dash: Basada en el eje horizontal o en la escala si no hay input
            float inputX = stateMachine.RB.linearVelocity.x; // Simplificación
            float dirX = stateMachine.transform.localScale.x;
            _dashDirection = new Vector2(dirX, 0).normalized;

            // Congelar gravedad
            _originalGravity = stateMachine.RB.gravityScale;
            stateMachine.RB.gravityScale = 0;
            stateMachine.RB.linearVelocity = _dashDirection * stats.dashSpeed;
        }

        public override void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                stateMachine.ChangeState(new AirborneState(stateMachine));
            }
        }

        public override void Exit()
        {
            // Restaurar gravedad
            stateMachine.RB.gravityScale = _originalGravity;
            
            // Conservar algo de inercia pero frenar un poco el dash al salir
            stateMachine.RB.linearVelocity *= 0.5f;
        }
    }
}
