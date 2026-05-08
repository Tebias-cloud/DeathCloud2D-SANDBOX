using UnityEngine;

namespace DeathCloud.Player.States
{
    using Core;

    public class HookedState : PlayerState
    {
        private Vector2 _anchorPoint;
        private Vector2 _anchorOffset;
        private Transform _targetTransform;
        private Rigidbody2D _targetRB;
        private float _tensionStress;
        private float _horizontalInput;
        private float _verticalInput;

        public HookedState(PlayerStateMachine stateMachine, Vector2 anchorPoint, Transform targetTransform = null) : base(stateMachine)
        {
            _anchorPoint = anchorPoint;
            _targetTransform = targetTransform;
            if (_targetTransform != null)
            {
                _targetRB = _targetTransform.GetComponent<Rigidbody2D>();
                _anchorOffset = _targetTransform.InverseTransformPoint(anchorPoint);
            }
        }

        public override void Enter()
        {
            input.MoveEvent += OnMove;
            input.JumpEvent += OnJump;
            input.GrappleCanceledEvent += OnGrappleReleased;
            input.DashEvent += OnDash;
            input.AttackEvent += OnAttack;

            stateMachine.Joint.enabled = true;
            
            if (_targetRB != null)
            {
                stateMachine.Joint.connectedBody = _targetRB;
                stateMachine.Joint.connectedAnchor = _anchorOffset;
            }
            else
            {
                stateMachine.Joint.connectedBody = null;
                stateMachine.Joint.connectedAnchor = _anchorPoint;
            }

            stateMachine.Joint.distance = Vector2.Distance(stateMachine.transform.position, GetCurrentAnchorPoint());
            
            stateMachine.Line.enabled = true;
            _tensionStress = 0f;
        }

        public override void Exit()
        {
            input.MoveEvent -= OnMove;
            input.JumpEvent -= OnJump;
            input.GrappleCanceledEvent -= OnGrappleReleased;
            input.DashEvent -= OnDash;
            input.AttackEvent -= OnAttack;

            stateMachine.Joint.enabled = false;
            stateMachine.Joint.connectedBody = null; // Limpiar conexión
            stateMachine.Line.enabled = false;
        }

        private void OnDash()
        {
            stateMachine.ChangeState(new DashState(stateMachine));
        }

        private void OnMove(Vector2 move)
        {
            _horizontalInput = move.x;
            _verticalInput = move.y;
        }

        private void OnJump()
        {
            ApplySpiderManJump();
            stateMachine.ChangeState(new AirborneState(stateMachine, true));
        }

        private void OnGrappleReleased()
        {
            Vector2 boost = stateMachine.RB.linearVelocity * 1.1f; 
            stateMachine.RB.linearVelocity = boost;
            stateMachine.ChangeState(new AirborneState(stateMachine));
        }

        private void OnAttack()
        {
            // Atacar sin soltar el gancho
            if (stateMachine.basicAttackSound != null && DeathCloud.Core.Audio.AudioManager.Instance != null)
            {
                DeathCloud.Core.Audio.AudioManager.Instance.PlaySFX(stateMachine.basicAttackSound);
            }

            // Ejecutar hitbox de ataque (mismo rango que AttackState)
            float lookDir = stateMachine.transform.localScale.x;
            Vector2 attackPoint = (Vector2)stateMachine.transform.position + new Vector2(lookDir * stats.attackRange * 0.5f, 0);
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint, stats.attackRange * 0.8f);

            foreach (var obj in hitObjects)
            {
                if (obj.transform.IsChildOf(stateMachine.transform)) continue;

                if (obj.CompareTag("Breakable"))
                {
                    if (stateMachine.glassBreakSound != null && DeathCloud.Core.Audio.AudioManager.Instance != null)
                    {
                        DeathCloud.Core.Audio.AudioManager.Instance.PlaySFX(stateMachine.glassBreakSound);
                    }
                    Object.Destroy(obj.gameObject);
                }
                else if (obj.CompareTag("Enemy"))
                {
                    if (obj.TryGetComponent(out DeathCloud.Core.Combat.IDamageable damageable))
                    {
                        damageable.TakeDamage(stats.attackDamage);
                    }
                    
                    if (obj.TryGetComponent(out Features.Combat.DummyTarget enemy))
                    {
                        enemy.ApplyStun(1.5f);
                    }
                }
            }
        }

        public override void Update()
        {
            UpdateLineAndTension();
            HandleClimbing();
        }

        public override void FixedUpdate()
        {
            if (_horizontalInput != 0)
            {
                stateMachine.RB.AddForce(new Vector2(_horizontalInput * stats.swingForce, 0));
            }

            if (stateMachine.RB.linearVelocity.magnitude > stats.maxSpeedLimit)
            {
                stateMachine.RB.linearVelocity = stateMachine.RB.linearVelocity.normalized * stats.maxSpeedLimit;
            }
        }

        private void UpdateLineAndTension()
        {
            Vector3 currentAnchor = GetCurrentAnchorPoint();
            stateMachine.Line.SetPosition(0, stateMachine.transform.position);
            stateMachine.Line.SetPosition(1, currentAnchor);

            float currentDist = Vector2.Distance(stateMachine.transform.position, currentAnchor);
            float tensionPercent = Mathf.Clamp01(currentDist / stats.maxHookDistance);
            float currentTimeToBreak = Mathf.Lerp(stats.snappingTimeSlack, stats.snappingTimeTense, tensionPercent);
            
            _tensionStress += Time.deltaTime / currentTimeToBreak;

            if (_tensionStress >= 1f)
            {
                stateMachine.ChangeState(new AirborneState(stateMachine));
            }
        }

        private void HandleClimbing()
        {
            if (_verticalInput != 0)
            {
                float climbSpeed = 10f; 
                stateMachine.Joint.distance -= _verticalInput * climbSpeed * Time.deltaTime;
                stateMachine.Joint.distance = Mathf.Clamp(stateMachine.Joint.distance, 1f, stats.maxHookDistance);
            }
        }

        private void ApplySpiderManJump()
        {
            Vector2 vel = stateMachine.RB.linearVelocity;
            bool isPerfect = vel.magnitude > (stats.maxSpeedLimit * 0.6f);
            
            Vector2 finalVel;
            if (isPerfect)
            {
                float verticalBoost = stats.verticalPopForce * (stats.perfectTimingMultiplier * 0.8f);
                finalVel = new Vector2(vel.x * stats.perfectTimingMultiplier, verticalBoost);
            }
            else
            {
                finalVel = new Vector2(vel.x, Mathf.Max(0, vel.y) + stats.normalJumpPop);
            }
 
            float maxLaunchSpeed = stats.maxSpeedLimit * 1.5f; 
            stateMachine.RB.linearVelocity = Vector2.ClampMagnitude(finalVel, maxLaunchSpeed);
        }

        private Vector3 GetCurrentAnchorPoint()
        {
            if (_targetTransform != null)
            {
                return _targetTransform.TransformPoint(_anchorOffset);
            }
            return _anchorPoint;
        }
    }
}
