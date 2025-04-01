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
        
        [SerializeField] private string parameterName;
        [SerializeField] private float parameterValue = 1f; 
        
        private float pausedValue = 0f;
       
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
            Cursor.lockState = CursorLockMode.None;
            AudioManager.instance.SetMusicParameter(parameterName, pausedValue);
            AudioManager.instance.SetAmbienceParameter(parameterName, pausedValue);
        }
        public void DeactivateMenu()
        {
            Time.timeScale = 1;
            pauseUI.SetActive(false);
            isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            AudioManager.instance.SetMusicParameter(parameterName, parameterValue);
            AudioManager.instance.SetAmbienceParameter(parameterName, parameterValue);
        }
        
        void Start()
        {
            if (!isPaused)
            {
                Time.timeScale = 1;
                pauseUI.SetActive(false);
                isPaused = false;
                AudioManager.instance.SetMusicParameter(parameterName, parameterValue);
                AudioManager.instance.SetAmbienceParameter(parameterName, parameterValue);
            }
        }
    }
}