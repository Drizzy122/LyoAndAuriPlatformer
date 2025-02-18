using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;

namespace Platformer
{
    public class PauseMenu : MonoBehaviour
    {

        // Reference to the PlayerControl class, which handles player input
        private PlayerInputActions playerInput;

        // Input action for the escape menu
        private InputAction menu;

        private EventInstance musicInstance;

        // Reference to the pause menu UI
        [SerializeField] private GameObject pauseUI;

        // Boolean to keep track of whether the game is paused or not
        [SerializeField] private bool isPaused = false;


        void Start()
        {
            if (!isPaused)
            {
                // Resume in-game time
                Time.timeScale = 1;
                // Unpause audio

                // Deactivate the pause menu UI
                pauseUI.SetActive(false);
                // Set isPaused to false
                isPaused = false;
            }
        }

        // Initialize playerControl when the script starts
        void Awake()
        {
            playerInput = new PlayerInputActions();
            musicInstance = AudioManager.instance.CreateEventInstance(FMODEvents.instance.music);
        }

        // Enable the menu action when the script is enabled
        private void OnEnable()
        {
            // Get the escape menu action from the PlayerControl class
            menu = playerInput.Menu.Escape;
            // Enable the escape menu action
            menu.Enable();

            // Add a callback method to be called when the escape menu action is performed
            menu.performed += Pause;
            // menu.canceled += Pause;
        }

        // Disable the menu action when the script is disabled
        private void OnDisable()
        {
            // Disable the escape menu action
            menu.Disable();
        }

        // Method called when the escape menu action is performed
        void Pause(InputAction.CallbackContext context)
        {
            // Toggle the isPaused boolean
            isPaused = !isPaused;
            // If the game is paused, activate the pause menu
            if (isPaused)
            {
                ActivateMenu();
            }
            // If the game is not paused, deactivate the pause menu
            else
            {
                DeactivateMenu();
            }
        }

        // Method to activate the pause menu
        void ActivateMenu()
        {
            // Stop in-game time
            Time.timeScale = 0;
            // Pause audio
            musicInstance.stop(STOP_MODE.ALLOWFADEOUT);
            // Activate the pause menu UI
            pauseUI.SetActive(true);
            // Cursor.lockState = CursorLockMode.None;
        }
        // Method to deactivate the pause menu
        public void DeactivateMenu()
        {
            // Resume in-game time
            Time.timeScale = 1;
            // Unpause audio
            AudioListener.pause = false;
            // Deactivate the pause menu UI
            pauseUI.SetActive(false);
            // Set isPaused to false
            isPaused = false;
            // Cursor.lockState = CursorLockMode.Locked;
        }
    }
}