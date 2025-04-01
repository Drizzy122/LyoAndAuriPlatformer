using UnityEngine;
using UnityEngine.UI;

namespace Platformer
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private Image totalhealthBar;
        [SerializeField] private Image currenthealthBar;

        // Adding a field to control the speed of the lerp
        [SerializeField] private float lerpSpeed = 5f;

        private void Start()
        {
            totalhealthBar.fillAmount = playerHealth.currentHealth / 100f;
        }

        private void Update()
        {
            // Target health percentage
            float targetFill = playerHealth.currentHealth / 100f;

            // Smoothly interpolate the current health bar
            currenthealthBar.fillAmount = Mathf.Lerp(currenthealthBar.fillAmount, targetFill, lerpSpeed * Time.deltaTime);
        }
    }
}