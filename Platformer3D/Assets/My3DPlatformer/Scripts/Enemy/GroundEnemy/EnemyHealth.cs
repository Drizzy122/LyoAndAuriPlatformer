using UnityEngine;

namespace Platformer
{
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private float startingHealth;
        public float currentHealth { get; private set; }
        private bool dead;
        
        public System.Action OnDeath; // Add a public event to notify death

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
                    OnDeath?.Invoke(); // Trigger death event
                }
            }
        }
    }
}