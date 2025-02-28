using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using FMODUnity;

namespace Platformer
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private float startingHealth;
        public float currentHealth { get; private set; }
        private bool dead;
        public System.Action OnDeath; // Add a public event to notify death
        public bool IsInvulnerable { get; private set; }
        //[Header("iFrames")] 
        //[SerializeField] private float iFramesDuration;
       // [SerializeField] private int numberOfFlashes;
       // public Renderer meshRenderer;
        
        private void Awake()
        {
            currentHealth = startingHealth;
          //  meshRenderer = GetComponent<Renderer>();
        }

        public void TakeDamage(float damage)
        {
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
            if (currentHealth > 0)
            {
                HandleDamage();
                //StartCoroutine(Invunerability());
            }
            else
            {
                HandleDeath();
            }
        }

        public void AddHealth(float value)
        {
            currentHealth = Mathf.Clamp(currentHealth + value, 0, startingHealth);
        }

        private void HandleDamage()
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerHurt, this.transform.position);
        }

        private void HandleDeath()
        {
            if (!dead)
            {
               
                AudioManager.instance.PlayOneShot(FMODEvents.instance.playerDeath, this.transform.position);
                
                GetComponent<PlayerController>().enabled = false;
                
                GameObject cinemachineGameObject = GameObject.Find("FreeLook Camera"); // Adjust the GameObject name
                if (cinemachineGameObject != null)
                {
                    cinemachineGameObject.SetActive(false);
                }

                dead = true;
                
                Invoke(nameof(RestartScene), 2f); // Restart scene after  seconds
                OnDeath?.Invoke(); // Trigger death event
                
                // GameEventsManager.instance.PlayerDeath();
            }
        }
        
        /*
        private IEnumerator Invunerability()
        {
            IsInvulnerable = true;
            Physics.IgnoreLayerCollision(10, 11, true); // Adjust layer numbers as needed
            for (int i = 0; i < numberOfFlashes; i++)
            {
                meshRenderer.material.color = new Color(1, 0, 0, 0.5f);
                yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
                meshRenderer.material.color = Color.white;
                yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            }

            Physics.IgnoreLayerCollision(10, 11, false);
            IsInvulnerable = false;
        }
        */

        private void RestartScene()
        {
            SceneManager.LoadScene("DeathScene"); // Replace "EndScreen" with the name of your end screen scene
        }
    }
}