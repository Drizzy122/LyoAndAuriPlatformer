using UnityEngine;
using UnityEngine.UI;

namespace Platformer
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private Image totalhealthBar;
        [SerializeField] private Image currenthealthBar;

        private void Start()
        {
            totalhealthBar.fillAmount = playerHealth.currentHealth / 100;
        }

        private void Update()
        {
            currenthealthBar.fillAmount = playerHealth.currentHealth / 100;
        }
    }
}