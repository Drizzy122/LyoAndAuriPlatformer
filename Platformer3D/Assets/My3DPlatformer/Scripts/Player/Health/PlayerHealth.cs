using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using System.Collections;
using Platformer;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    private bool dead;
    
    [Header("iFrames")]
    public bool IsInvulnerable { get; private set; }

    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    public Renderer meshRenderer;
    
    
    //private Animator animator;
    private void Awake()
    { 
        currentHealth = startingHealth;
     //   animator = GetComponent<Animator>();
     meshRenderer = GetComponent<Renderer>();
    }
    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
        if (currentHealth > 0)
        {
            HandleDamage();
            StartCoroutine(Invunerability());
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
       // animator.SetTrigger("isHurt");
    }
    private void HandleDeath()
    {
        if (!dead)
        {
            //animator.SetTrigger("isDead");
            GetComponent<PlayerController>().enabled = false;
            GameObject cinemachineGameObject = GameObject.Find("FreeLook Camera"); // Adjust the GameObject name
            if (cinemachineGameObject != null)
            {
                cinemachineGameObject.SetActive(false);
            }
            dead = true;
            Invoke(nameof(RestartScene), 5f); // Restart scene after  seconds
           // GameEventsManager.instance.PlayerDeath();
        }
    }
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
    private void RestartScene()
    {
        SceneManager.LoadScene("TestingPlayGroundScene"); // Replace "EndScreen" with the name of your end screen scene
    }
}