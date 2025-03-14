using UnityEngine;
using UnityEngine.InputSystem;

namespace Platformer
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputManager : MonoBehaviour
    {
        public void SubmitPressed(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                GameEventsManager.instance.inputEvents.SubmitPressed();
            }
        }
    }
}