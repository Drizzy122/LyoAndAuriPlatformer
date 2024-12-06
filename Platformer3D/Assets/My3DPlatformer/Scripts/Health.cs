using UnityEngine;
using UnityEngine.SceneManagement; // For resetting the scene

namespace Platformer {
    public class Health : MonoBehaviour {
        [SerializeField] int maxHealth = 100;
        [SerializeField] FloatEventChannel playerHealthChannel;

        int currentHealth;

        public bool IsDead => currentHealth <= 0;

        void Awake() {
            currentHealth = maxHealth;
        }

        void Start() {
            PublishHealthPercentage();
        }

        public void TakeDamage(int damage) {
            currentHealth -= damage;
            PublishHealthPercentage();

            if (IsDead) {
                HandleDeath();
            }
        }

        void HandleDeath() {
            if (gameObject.CompareTag("Enemy")) {
                // Destroy the enemy game object
                Destroy(gameObject);
            } else if (gameObject.CompareTag("Player")) {
                // Reset the scene if it's the player
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        void PublishHealthPercentage() {
            if (playerHealthChannel != null)
                playerHealthChannel.Invoke(currentHealth / (float)maxHealth);
        }
    }
}