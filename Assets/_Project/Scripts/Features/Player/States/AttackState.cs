using UnityEngine;

namespace DeathCloud.Player.States
{
    using Core;
    using DeathCloud.Core.Combat;

    public class AttackState : PlayerState 
    {
        private float attackDuration = 0.4f; // Duración temporal hasta tener animaciones
        private float timePassed;

        public AttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            Debug.Log("[AttackState] Atacando...");
            timePassed = 0f;
            
            // Frenar al jugador horizontalmente al atacar
            stateMachine.RB.linearVelocity = new Vector2(0, stateMachine.RB.linearVelocity.y);

            ExecuteHitbox();
        }

        private void ExecuteHitbox()
        {
            // Posición frente al jugador según su escala local (dirección)
            float lookDir = stateMachine.transform.localScale.x;
            Vector2 attackPoint = (Vector2)stateMachine.transform.position + new Vector2(lookDir * stats.attackRange * 0.5f, 0);
            
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint, stats.attackRange * 0.5f, stats.damageableLayer);

            foreach (var obj in hitObjects)
            {
                if (obj.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(stats.attackDamage);
                    Debug.Log($"[AttackState] Golpeado: {obj.name}");
                }
            }
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
                if (stateMachine.IsGrounded())
                {
                    stateMachine.ChangeState(new GroundedState(stateMachine));
                }
                else
                {
                    stateMachine.ChangeState(new AirborneState(stateMachine));
                }
            }
        }
    }
}
