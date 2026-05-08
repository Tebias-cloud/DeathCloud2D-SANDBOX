using UnityEngine;
using UnityEngine.InputSystem;

namespace DeathCloud.Player.States
{
    using Core;
    using DeathCloud.Core.Audio;
    using DeathCloud.Features.Combat;

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
            Vector2 mousePos;
            if (Mouse.current != null)
            {
                mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            }
            else
            {
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            Vector2 direction = (mousePos - (Vector2)stateMachine.transform.position).normalized;
            Debug.DrawRay(stateMachine.transform.position, direction * stats.maxHookDistance, Color.red, 1f);
            
            // VOLVEMOS A RAYCAST ALL QUE FUNCIONABA MEJOR
            RaycastHit2D[] hits = Physics2D.RaycastAll(stateMachine.transform.position, direction, stats.maxHookDistance);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            
            RaycastHit2D hit = new RaycastHit2D();

            foreach (var h in hits)
            {
                if (h.collider == null || h.transform.IsChildOf(stateMachine.transform)) continue;

                // Si es algo rompible, lo rompemos e ignoramos el resto del rayo
                if (h.collider.CompareTag("Breakable"))
                {
                    if (stateMachine.glassBreakSound != null && DeathCloud.Core.Audio.AudioManager.Instance != null)
                    {
                        DeathCloud.Core.Audio.AudioManager.Instance.PlaySFX(stateMachine.glassBreakSound);
                    }
                    Object.Destroy(h.collider.gameObject);
                    return;
                }

                // Si es un enemigo, lo aturdimos, pero SEGUIMOS buscando por si hay un collider sólido detrás o si él mismo tiene uno
                if (h.collider.CompareTag("Enemy"))
                {
                    if (h.collider.TryGetComponent(out Features.Combat.DummyTarget enemy))
                    {
                        enemy.ApplyStun(1.5f);
                    }

                    if (stateMachine.hookHitEnemySound != null && DeathCloud.Core.Audio.AudioManager.Instance != null)
                    {
                        DeathCloud.Core.Audio.AudioManager.Instance.PlaySFX(stateMachine.hookHitEnemySound);
                    }
                    
                    // Si el enemigo es un trigger, seguimos buscando para el anclaje físico
                    // Si no es trigger, lo usamos como anclaje y paramos.
                    if (!h.collider.isTrigger)
                    {
                        hit = h;
                        break;
                    }
                    else if (hit.collider == null) // Guardamos el trigger como backup si no hay nada sólido aún
                    {
                        hit = h;
                    }
                }
                else if (!h.collider.isTrigger) // Superficie sólida normal
                {
                    hit = h;
                    break;
                }
            }

            if (hit.collider != null)
            {
                if (stateMachine.hookLaunchSound != null && DeathCloud.Core.Audio.AudioManager.Instance != null)
                {
                    DeathCloud.Core.Audio.AudioManager.Instance.PlaySFX(stateMachine.hookLaunchSound);
                }

                // Actualizar línea inmediatamente para evitar el "parpadeo" o que parezca que atraviesa
                stateMachine.Line.enabled = true;
                stateMachine.Line.SetPosition(0, stateMachine.transform.position);
                stateMachine.Line.SetPosition(1, hit.point);

                stateMachine.ChangeState(new HookedState(stateMachine, hit.point, hit.collider.transform));
            }
        }

        private void OnJump()
        {
            stateMachine.RB.linearVelocity = new Vector2(stateMachine.RB.linearVelocity.x, stats.jumpForce);
            stateMachine.ChangeState(new AirborneState(stateMachine));
        }

        public override void Update()
        {
            if (!stateMachine.IsGrounded())
            {
                stateMachine.ChangeState(new AirborneState(stateMachine));
                return;
            }

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
