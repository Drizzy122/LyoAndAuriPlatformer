using UnityEngine;

namespace Platformer
{
    public class Traps : MonoBehaviour
    {
        [SerializeField] private float damage;
        [SerializeField] private bool isWater;
        
        private void OnTriggerEnter(Collider collision)
        {
            if (collision.tag == "Player")
            {
                if (isWater) collision.GetComponent<PlayerController>().drowning = true;
                collision.GetComponent<PlayerHealth>().TakeDamage(damage);
            }
            else if (collision.tag == "Enemy")
            {
                collision.GetComponent<EnemyHealth>().TakeDamage(100,0);
            }
        }
    }
}
