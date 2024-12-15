using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

namespace Platformer
{
    public class EndGameTrigger : MonoBehaviour
    {
        [SerializeField] private string sceneName; // Exposed in Inspector for specifying scene name

        // Check for collision with the player
        private void OnTriggerEnter(Collider other)
        {
            // Assuming "Player" is the tag assigned to your player character
            if (other.CompareTag("Player"))
            {
                // Load the specified scene from the Inspector
                if (!string.IsNullOrEmpty(sceneName)) // Ensure a scene name is provided
                {
                    SceneManager.LoadScene(sceneName);
                }
                else
                {
                    Debug.LogWarning("Scene name is empty! Please assign a scene name in the Inspector.");
                }
            }
        }
    }
}