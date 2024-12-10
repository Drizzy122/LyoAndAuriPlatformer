using UnityEngine;

namespace Platformer
{
    public class Traps : MonoBehaviour
    {
        [SerializeField] private float damage;
        
        private void OnTriggerEnter(Collider collision)
        {
            if (collision.tag == "Player")
            {
                collision.GetComponent<PlayerHealth>().TakeDamage(damage);
            }
        }
    }
}
