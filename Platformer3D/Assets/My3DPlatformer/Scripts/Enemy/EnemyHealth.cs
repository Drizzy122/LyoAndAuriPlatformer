using UnityEngine;

namespace Platformer
{
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private float startingHealth;
        public float currentHealth { get; private set; }
        private bool dead;

        private void Awake()
        {
            currentHealth = startingHealth;
        }

        public void TakeDamage(float damage)
        {
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
            if (currentHealth > 0)
            {
                //noop
                AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyHurt, this.transform.position);
            }
            else
            {
                if (!dead)
                {
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyDeath, this.transform.position);
                    dead = true;
                    Destroy(gameObject, 1f); // Destroy enemy object after 1 seconds
                }
            }
        }
    }
}