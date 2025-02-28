using UnityEngine;
using UnityEngine.InputSystem;

namespace Platformer
{
    public class PauseMenu : MonoBehaviour
    {
        private PlayerInputActions playerInput;
        private InputAction menu;
        
        [SerializeField] private GameObject pauseUI;
        [SerializeField] private bool isPaused = false;
        void Start()
        {
            if (!isPaused)
            {
                Time.timeScale = 1;
                pauseUI.SetActive(false);
                isPaused = false;
            }
        }
        void Awake()
        {
            playerInput = new PlayerInputActions();
        }
        private void OnEnable()
        {
            menu = playerInput.Menu.Escape;
            menu.Enable();
            menu.performed += Pause; 
            //menu.canceled += Pause;
        }
        private void OnDisable()
        {
            menu.Disable();
        }
        void Pause(InputAction.CallbackContext context)
        {
            isPaused = !isPaused;
            if (isPaused)
            {
                ActivateMenu();
            }
            else
            {
                DeactivateMenu();
            }
        }
        void ActivateMenu()
        {
            Time.timeScale = 0;
            pauseUI.SetActive(true);
            //Cursor.lockState = CursorLockMode.None;
        }
        public void DeactivateMenu()
        {
            Time.timeScale = 1;
            pauseUI.SetActive(false);
            isPaused = false;
            //Cursor.lockState = CursorLockMode.Locked;
        }
    }
}