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
            RB = GetComponent<Rigidbody2D>();
            Collider = GetComponent<Collider2D>();
            Joint = GetComponent<DistanceJoint2D>();
            Line = GetComponent<LineRenderer>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (IsOwner)
            {
                var virtualCamera = Object.FindAnyObjectByType<CinemachineCamera>();
                if (virtualCamera != null)
                {
                    // Si asignaste un CameraTarget en el inspector, lo usa. Si no, usa el centro del jugador.
                    virtualCamera.Follow = _cameraTarget != null ? _cameraTarget : transform;
                }
                else
                {
                    UnityEngine.Debug.LogWarning("No se encontró una CinemachineCamera en la escena.");
                }
            }
        }

        private void Start()
        {
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
            
            // Debug para saber en qué estado estamos
            // Debug.Log($"Cambio de estado a: {newState.GetType().Name}");
        }
    }
}
