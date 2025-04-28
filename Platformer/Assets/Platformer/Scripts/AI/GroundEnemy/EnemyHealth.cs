using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace Platformer
{
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private float startingHealth;
        public float currentHealth { get; private set; }
        private bool dead;
        
        public System.Action OnDeath; // Add a public event to notify death

        [SerializeField] private GameObject healthPrefab;
        [SerializeField] private GameObject XPOrbPrefab;
        [SerializeField] private int dropCount = 5; // Number of prefabs to spawn
        [SerializeField] private float spawnRadius = 1f; // 

        public SkinnedMeshRenderer skinnedMesh;
        private Material[] skinnedMaterials;
        [SerializeField] private float dissolveRate = 0.0125f;
        [SerializeField] private float refreshRate = 0.025f;
        public VisualEffect VFXGraph;
        

        private void Awake()
        {
            if (skinnedMesh != null)
                skinnedMaterials = skinnedMesh.materials;
            currentHealth = startingHealth;
        }

        public void TakeDamage(float damage, float knockBackTime)
        {
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
            if (currentHealth > 0)
            {
                if (TryGetComponent(out Enemy enemy))
                {
                    enemy.knockbackTimer = knockBackTime;
                }
                else if(TryGetComponent(out FlyingEnemy flyingEnemy))
                {
                    flyingEnemy.knockbackTimer = knockBackTime;
                }

                AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyHurt, this.transform.position);
            }
            else
            {
                if (!dead)
                {
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyDeath, this.transform.position);
                    dead = true;
                   HandleDeath();
                   StartCoroutine(DissolveCo());
                }
            }
        }
        private void HandleDeath()
        {
            // Trigger death event
            OnDeath?.Invoke(); 
            GameEventsManager.instance.enemyEvents.EnemyDeath();
            OrbDrop();
            StartCoroutine(DissolveCo());
    
        }
        
        public void OrbDrop()
        {
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
                    Instantiate(XPOrbPrefab, spawnPosition, Quaternion.identity);
                    
                }
            }
        }
        IEnumerator DissolveCo()
        {
            if (VFXGraph != null)
            {
                VFXGraph.Play();
            }
                {if (skinnedMaterials.Length > 0)
            {
                float counter = 0;
                while (skinnedMaterials[0].GetFloat("_DissolveAmount") < 1)
                {
                    counter += dissolveRate;
                    for (int i = 0; i < skinnedMaterials.Length; i++)
                    {
                        skinnedMaterials[i].SetFloat("_DissolveAmount", counter);
                    }
                    yield return new WaitForSeconds(refreshRate);
                }
            }}
        }
    }
}