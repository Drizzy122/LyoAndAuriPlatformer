using System.Collections;
using Cinemachine;
using KBCore.Refs;
using UnityEngine;

namespace Platformer {
    public class CameraManager : ValidatedMonoBehaviour {
        [Header("References")]
        [SerializeField, Anywhere] InputReader input;
        [SerializeField, Anywhere] CinemachineFreeLook freeLookVCam;

        [Header("Settings")] 
        [SerializeField, Range(0.5f, 3f)] float speedMultiplier = 1f;

        bool cameraMovementLock;

        void OnEnable() {
            input.Look += OnLook;
            // Lock and hide the cursor when the component is enabled
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        void OnDisable() {
            input.Look -= OnLook;
            // Unlock and show the cursor when the component is disabled
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        void OnLook(Vector2 cameraMovement, bool isDeviceMouse) {
            if (cameraMovementLock) return;

            // If the device is mouse, use fixedDeltaTime, otherwise use deltaTime
            float deviceMultiplier = isDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime;
            
            // Set the camera axis values
            freeLookVCam.m_XAxis.m_InputAxisValue = cameraMovement.x * speedMultiplier * deviceMultiplier;
            freeLookVCam.m_YAxis.m_InputAxisValue = cameraMovement.y * speedMultiplier * deviceMultiplier;
        }
    }
}