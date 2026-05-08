using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;
using DeathCloud.Core.Input;
using DeathCloud.Player;
using DeathCloud.Player.States;

namespace DeathCloud.Player.Core
{
    public interface IState
    {
        void Enter();
        void Update();
        void FixedUpdate();
        void Exit();
    }

    public abstract class PlayerState : IState
    {
        protected PlayerStateMachine stateMachine;
        protected PlayerStatsSO stats;
        protected InputReader input;

        public PlayerState(PlayerStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
            this.stats = stateMachine.Stats;
            this.input = stateMachine.Input;
        }

        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void Exit() { }
    }

    public class PlayerStateMachine : NetworkBehaviour
    {
        [SerializeField] private PlayerStatsSO _stats;
        [SerializeField] private InputReader _input;
        [SerializeField] private Transform _cameraTarget; // Agregado para centrar la cámara

        public PlayerStatsSO Stats => _stats;
        public InputReader Input => _input;
        public Rigidbody2D RB { get; private set; }
        public Collider2D Collider { get; private set; }
        public DistanceJoint2D Joint { get; private set; }
        public LineRenderer Line { get; private set; }

        private IState _currentState;
        private float _jumpBufferTimer;

        private void Awake()
        {
            RB = GetComponentInChildren<Rigidbody2D>();
            
            // Buscar el collider físico real (ignorando hitboxes que suelen ser triggers)
            Collider2D[] allColliders = GetComponentsInChildren<Collider2D>();
            foreach(var col in allColliders)
            {
                if (!col.isTrigger)
                {
                    Collider = col;
                    break;
                }
            }

            Joint = GetComponentInChildren<DistanceJoint2D>();
            Line = GetComponentInChildren<LineRenderer>();

            if (_stats == null) Debug.LogError($"[PlayerStateMachine] ERROR: StatsSO no está asignado en {gameObject.name}.");
            if (_input == null) Debug.LogError($"[PlayerStateMachine] ERROR: InputReader no está asignado en {gameObject.name}.");
            if (Collider == null) Debug.LogError($"[PlayerStateMachine] ERROR: No se encontró Collider2D físico en {gameObject.name}.");
            if (RB == null) Debug.LogError($"[PlayerStateMachine] ERROR: No se encontró Rigidbody2D en {gameObject.name}.");
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (IsOwner)
            {
                Debug.Log($"[PlayerStateMachine] Local Player Spawned: {gameObject.name} (NetId: {NetworkObjectId})");
                var virtualCamera = Object.FindAnyObjectByType<CinemachineCamera>();
                if (virtualCamera != null)
                {
                    virtualCamera.Follow = _cameraTarget != null ? _cameraTarget : transform;
                }
            }
        }

        private void Start()
        {
            if (!IsOwner) return;
            
            if (_stats == null || _input == null)
            {
                Debug.LogError($"[PlayerStateMachine] Abortando inicialización de estados en {gameObject.name} por falta de referencias.");
                return;
            }

            ChangeState(new GroundedState(this));
        }

        private void Update()
        {
            if (!IsOwner) return;
            if (_jumpBufferTimer > 0) _jumpBufferTimer -= Time.deltaTime;
            _currentState?.Update();
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;
            _currentState?.FixedUpdate();
        }

        public void SetJumpBuffer() => _jumpBufferTimer = Stats.jumpBufferTime;
        public bool HasJumpBuffer => _jumpBufferTimer > 0;
        public void ConsumeJumpBuffer() => _jumpBufferTimer = 0;

        public void ChangeState(IState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState?.Enter();
        }

        // ==========================================
        // DETECCIÓN FÍSICA CENTRALIZADA (FASE 3)
        // ==========================================
        
        public bool IsGrounded()
        {
            if (Collider == null) return false;
            
            // Caja en los pies: 1.2x el ancho del collider para máxima estabilidad en bordes
            Vector2 boxSize = new Vector2(Collider.bounds.size.x * 1.2f, 0.3f); 
            Vector2 boxCenter = new Vector2(Collider.bounds.center.x, Collider.bounds.min.y + 0.05f);
            
            return Physics2D.OverlapBox(boxCenter, boxSize, 0f, Stats.groundLayer);
        }

        public bool IsWalled(float directionX)
        {
            if (Collider == null) return false;
            
            // Caja lateral: detecta contacto con paredes en la dirección del movimiento
            Vector2 boxSize = new Vector2(0.3f, Collider.bounds.size.y * 0.8f);
            Vector2 boxCenter = new Vector2(Collider.bounds.center.x + (Collider.bounds.extents.x * directionX), Collider.bounds.center.y);
            
            return Physics2D.OverlapBox(boxCenter, boxSize, 0f, Stats.groundLayer);
        }
    }
}
