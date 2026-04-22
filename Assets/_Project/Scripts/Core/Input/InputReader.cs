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
            MoveEvent.Invoke(context.ReadValue<Vector2>());
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                JumpEvent.Invoke();
            if (context.phase == InputActionPhase.Canceled)
                JumpCanceledEvent.Invoke();
        }

        public void OnGrapple(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                GrappleEvent.Invoke();
            if (context.phase == InputActionPhase.Canceled)
                GrappleCanceledEvent.Invoke();
        }

        // Implementación de otros métodos de la interfaz (vacíos por ahora si no se usan)
        public void OnLook(InputAction.CallbackContext context) { }
        public void OnAttack(InputAction.CallbackContext context) { }
        public void OnInteract(InputAction.CallbackContext context) { }
        public void OnCrouch(InputAction.CallbackContext context) { }
        public void OnPrevious(InputAction.CallbackContext context) { }
        public void OnNext(InputAction.CallbackContext context) { }
        public void OnSprint(InputAction.CallbackContext context) 
        {
            if (context.phase == InputActionPhase.Performed)
                DashEvent.Invoke();
        }
    }
}
