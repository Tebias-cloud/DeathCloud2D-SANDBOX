using UnityEngine;
using UnityEngine.InputSystem;

namespace DeathCloud.Player.States
{
    using Core;
    using DeathCloud.Core.Audio;
    using DeathCloud.Features.Combat;

    public class AirborneState : PlayerState
    {
        private float _horizontalInput;
        private float _coyoteTimer;
        private bool _hasJumped;

        public AirborneState(PlayerStateMachine stateMachine, bool isJumping = false) : base(stateMachine)
        {
            _hasJumped = isJumping;
            _coyoteTimer = isJumping ? 0 : 0.2f; 
        }

        public override void Enter()
        {
            input.MoveEvent += OnMove;
            input.GrappleEvent += OnTryGrapple;
            input.JumpEvent += OnJumpInput;
            input.DashEvent += OnDashInput;
            input.AttackEvent += OnAttackPressed;

            _horizontalInput = input.MoveValue.x;
        }

        public override void Exit()
        {
            input.MoveEvent -= OnMove;
            input.GrappleEvent -= OnTryGrapple;
            input.JumpEvent -= OnJumpInput;
            input.DashEvent -= OnDashInput;
            input.AttackEvent -= OnAttackPressed;
        }

        private void OnAttackPressed()
        {
            stateMachine.ChangeState(new AttackState(stateMachine));
        }

        private void OnMove(Vector2 move)
        {
            _horizontalInput = move.x;
        }

        private void OnJumpInput()
        {
            if (!_hasJumped && _coyoteTimer > 0)
            {
                stateMachine.RB.linearVelocity = new Vector2(stateMachine.RB.linearVelocity.x, stats.jumpForce);
                _hasJumped = true;
                _coyoteTimer = 0;
            }
            else
            {
                stateMachine.SetJumpBuffer();
            }
        }

        private void OnDashInput()
        {
            stateMachine.ChangeState(new DashState(stateMachine));
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

        public override void Update()
        {
            if (_coyoteTimer > 0) _coyoteTimer -= Time.deltaTime;

            if (stateMachine.IsGrounded() && stateMachine.RB.linearVelocity.y <= 0.1f)
            {
                stateMachine.ChangeState(new GroundedState(stateMachine));
                return;
            }

            if (stateMachine.IsWalled(_horizontalInput) && _horizontalInput != 0)
            {
                stateMachine.ChangeState(new WallState(stateMachine));
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
