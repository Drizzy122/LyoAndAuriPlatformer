using UnityEngine;

namespace Platformer
{
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private float startingHealth;
        public float currentHealth { get; private set; }
        private bool dead;
        
        public System.Action OnDeath; // Add a public event to notify death

        [SerializeField] private GameObject healthPrefab;
        [SerializeField] private int dropCount = 5; // Number of prefabs to spawn
        [SerializeField] private float spawnRadius = 1f; // 

        private void Awake()
        {
            currentHealth = startingHealth;
        }

        public void TakeDamage(float damage)
        {
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
            if (currentHealth > 0)
            {
                
                AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyHurt, this.transform.position);
            }
            else
            {
                if (!dead)
                {
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyDeath, this.transform.position);
                    dead = true;
                   HandleDeath();
                }
            }
        }
        private void HandleDeath()
        {
            // Trigger death event
            OnDeath?.Invoke(); 
            GameEventsManager.instance.EnemyDeath();
            // Drop the prefab
            // Spawn multiple prefabs
            if (healthPrefab != null && dropCount > 0)
            {
                for (int i = 0; i < dropCount; i++)
                {
                    // Generate a random position within the spawn radius
                    Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
                    randomOffset.y = 0.4f; // Keep it on the same horizontal plane
                    Vector3 spawnPosition = transform.position + randomOffset;

                    // Instantiate the prefab at the random position
                    Instantiate(healthPrefab, spawnPosition, Quaternion.identity);
                }
            }

        }

        
    }
}