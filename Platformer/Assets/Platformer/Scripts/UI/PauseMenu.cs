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
            AudioManager.instance.PlayOneShot(FMODEvents.instance.uiopen, this.transform.position);
           // LeanTween.scale(pauseUI, new Vector3(1, 1, 1), 0.5f).setEase(LeanTweenType.easeOutBack);

        }
        public void DeactivateMenu()
        {
            Time.timeScale = 1;
            pauseUI.SetActive(false);
            isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            AudioManager.instance.SetMusicParameter(parameterName, parameterValue);
            AudioManager.instance.SetAmbienceParameter(parameterName, parameterValue);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.uiclose, this.transform.position);
           // LeanTween.scale(pauseUI, new Vector3(0, 0, 0), 0.5f).setEase(LeanTweenType.easeOutBack);
   
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