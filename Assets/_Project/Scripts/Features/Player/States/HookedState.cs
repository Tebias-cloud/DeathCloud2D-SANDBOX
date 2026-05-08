using UnityEngine;

namespace DeathCloud.Player.States
{
    using Core;

    public class HookedState : PlayerState
    {
        private Vector2 _anchorPoint;
        private float _tensionStress;
        private float _horizontalInput;
        private float _verticalInput;

        public HookedState(PlayerStateMachine stateMachine, Vector2 anchorPoint) : base(stateMachine)
        {
            _anchorPoint = anchorPoint;
        }

        public override void Enter()
        {
            input.MoveEvent += OnMove;
            input.JumpEvent += OnJump;
            input.GrappleCanceledEvent += OnGrappleReleased;
            input.DashEvent += OnDash;

            stateMachine.Joint.enabled = true;
            stateMachine.Joint.connectedAnchor = _anchorPoint;
            stateMachine.Joint.distance = Vector2.Distance(stateMachine.transform.position, _anchorPoint);
            
            stateMachine.Line.enabled = true;
            _tensionStress = 0f;
        }

        public override void Exit()
        {
            input.MoveEvent -= OnMove;
            input.JumpEvent -= OnJump;
            input.GrappleCanceledEvent -= OnGrappleReleased;
            input.DashEvent -= OnDash;

            stateMachine.Joint.enabled = false;
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
            // Tirachinas: Al soltar el botón, conservamos la velocidad actual
            // pero le damos un pequeño empujón extra si estamos moviéndonos rápido
            Vector2 boost = stateMachine.RB.linearVelocity * 1.1f; 
            stateMachine.RB.linearVelocity = boost;
            
            stateMachine.ChangeState(new AirborneState(stateMachine));
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

            // Cap de velocidad
            if (stateMachine.RB.linearVelocity.magnitude > stats.maxSpeedLimit)
            {
                stateMachine.RB.linearVelocity = stateMachine.RB.linearVelocity.normalized * stats.maxSpeedLimit;
            }
        }

        private void UpdateLineAndTension()
        {
            stateMachine.Line.SetPosition(0, stateMachine.transform.position);
            stateMachine.Line.SetPosition(1, _anchorPoint);

            // Lógica de tensión (Copiada del original)
            float currentDist = Vector2.Distance(stateMachine.transform.position, _anchorPoint);
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
                float climbSpeed = 10f; // Valor por defecto ajustable
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
                finalVel = new Vector2(vel.x * stats.perfectTimingMultiplier, stats.verticalPopForce * stats.perfectTimingMultiplier);
            }
            else
            {
                finalVel = new Vector2(vel.x, Mathf.Max(0, vel.y) + stats.normalJumpPop);
            }

            // Clamp final velocity to avoid "explosive" launches
            float maxLaunchSpeed = stats.maxSpeedLimit * 2f; 
            stateMachine.RB.linearVelocity = Vector2.ClampMagnitude(finalVel, maxLaunchSpeed);
        }
    }
}
