using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace DeathCloud.Core.Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "DeathCloud/Core/Input Reader")]
    public class InputReader : ScriptableObject, InputSystem_Actions.IPlayerActions
    {
        public event UnityAction<Vector2> MoveEvent = delegate { };
        public event UnityAction JumpEvent = delegate { };
        public event UnityAction JumpCanceledEvent = delegate { };
        public event UnityAction GrappleEvent = delegate { };
        public event UnityAction GrappleCanceledEvent = delegate { };
        public event UnityAction DashEvent = delegate { };
        public event UnityAction AttackEvent = delegate { };
        
        public bool IsJumpHeld { get; private set; }
        public Vector2 MoveValue { get; private set; }

        private InputSystem_Actions _inputActions;

        private void OnEnable()
        {
            if (_inputActions == null)
            {
                _inputActions = new InputSystem_Actions();
                _inputActions.Player.SetCallbacks(this);
            }
            _inputActions.Player.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Player.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveValue = context.ReadValue<Vector2>();
            MoveEvent.Invoke(MoveValue);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                IsJumpHeld = true;
                JumpEvent.Invoke();
            }
            else if (context.canceled)
            {
                IsJumpHeld = false;
                JumpCanceledEvent.Invoke();
            }
        }

        public void OnGrapple(InputAction.CallbackContext context)
        {
            if (context.started)
                GrappleEvent.Invoke();
            else if (context.canceled)
                GrappleCanceledEvent.Invoke();
        }

        public void OnAttack(InputAction.CallbackContext context) 
        {
            if (context.started)
                AttackEvent.Invoke();
        }
        public void OnLook(InputAction.CallbackContext context)
        {
            // No se usa en este proyecto 2D
        }

        public void OnInteract(InputAction.CallbackContext context) { }
        public void OnCrouch(InputAction.CallbackContext context) { }
        public void OnPrevious(InputAction.CallbackContext context) { }
        public void OnNext(InputAction.CallbackContext context) { }
        public void OnNewaction(InputAction.CallbackContext context) { }
        public void OnSprint(InputAction.CallbackContext context) 
        {
            if (context.phase == InputActionPhase.Performed)
                DashEvent.Invoke();
        }
    }
}
