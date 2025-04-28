using Unity.Cinemachine;
using KBCore.Refs;
using UnityEngine;

namespace Platformer
{
    public class CameraManager : ValidatedMonoBehaviour
    {
        
        [Header("References")] [SerializeField, Anywhere]
        InputReader input;

        [SerializeField, Anywhere] CinemachineOrbitalFollow freeLookCam;
        
        void OnEnable()
        {
            // Lock and hide the cursor when the component is enabled
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            // Enable Cinemachine camera
            if (freeLookCam != null)
            {
                freeLookCam.gameObject.SetActive(true);
            }

        }

        void OnDisable()
        {
            // Unlock and show the cursor when the component is disabled
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            // Disable Cinemachine camera
            if (freeLookCam != null)
            {
                freeLookCam.gameObject.SetActive(false);
            }

        }
    }
}