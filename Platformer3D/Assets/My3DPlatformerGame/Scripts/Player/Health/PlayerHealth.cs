using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    private bool dead;

    AudioManager audioManager;
    private Animator animator;
    private void Awake()
    { 
        currentHealth = startingHealth;
        animator = GetComponent<Animator>();
       // audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
        if (currentHealth > 0)
        {
            HandleDamage();
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
        animator.SetTrigger("isHurt");
        audioManager.PlaySFX(audioManager.hurt);
    }
    private void HandleDeath()
    {
        if (!dead)
        {
            animator.SetTrigger("isDead");
            audioManager.PlaySFX(audioManager.death);
            GetComponent<PlayerMovement>().enabled = false;
            GameObject cinemachineGameObject = GameObject.Find("FreeLook Camera"); // Adjust the GameObject name
            if (cinemachineGameObject != null)
            {
                cinemachineGameObject.SetActive(false);
            }
            dead = true;
            Invoke(nameof(RestartScene), 5f); // Restart scene after  seconds
            GameEventsManager.instance.PlayerDeath();
        }
    }
    private void RestartScene()
    {
        SceneManager.LoadScene("PlayGroundScene"); // Replace "EndScreen" with the name of your end screen scene
    }
}