using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputActions;

namespace Platformer
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Platformer/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions
    {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<Vector2, bool> Look = delegate { };
        public event UnityAction EnableMouseControlCamera = delegate { };
        public event UnityAction DisableMouseControlCamera = delegate { };
        public event UnityAction<bool> Jump = delegate { };
        public event UnityAction<bool> Dash = delegate { };
        public event UnityAction<bool> Echo = delegate { };
        public event UnityAction<bool> Wallclimb = delegate { };
        public event UnityAction<bool> Glide = delegate { };
        public event UnityAction Attack = delegate { };
        public event UnityAction SpinAttack = delegate { };
        public event UnityAction<bool> interact = delegate {  }; 
        
        PlayerInputActions inputActions;
        public Vector3 Direction => inputActions.Player.Move.ReadValue<Vector2>();

        void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerInputActions();
                inputActions.Player.SetCallbacks(this);
            }
        }

        public void EnablePlayerActions()
        {
            inputActions.Enable();
        }
        
        public void OnInteract(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    interact.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    interact.Invoke(false);
                    break;
            }
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            Move.Invoke(context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
        }
        bool IsDeviceMouse(InputAction.CallbackContext context)
        {
            return context.control.device.name == "Mouse";
        }
        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                Attack.Invoke();
            }
        }
        
        public void OnSpinAttack(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                SpinAttack.Invoke();
            }
        }

        public void OnMouseControlCamera(InputAction.CallbackContext context)
        {
            switch (context.action.phase)
            {
                case InputActionPhase.Started:
                    EnableMouseControlCamera.Invoke();
                    break;
                case InputActionPhase.Canceled:
                    DisableMouseControlCamera.Invoke();
                    break;
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                Jump.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Jump.Invoke(false);
                    break;
            }
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Dash.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Dash.Invoke(false);
                    break;
            }
        }
        public void OnEcho(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Echo.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Echo.Invoke(false);
                    break;
            }
        }

        public void OnWallclimb(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Wallclimb.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Wallclimb.Invoke(false);
                    break;
            }
        }

        public void OnGlide(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Glide.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Glide.Invoke(false);
                    break;
            }
        }
    }
}