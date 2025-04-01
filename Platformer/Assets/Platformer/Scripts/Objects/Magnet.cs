using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class Magnet : MonoBehaviour
    {
        public float attractorStrength = 10f;
        public float attractorRange = 10f;

        void FixedUpdate()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attractorRange);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Collectible"))
                {
                    Vector3 forceDirenction = transform.position - hitCollider.transform.position;
                    hitCollider.GetComponent<Rigidbody>().AddForce(forceDirenction.normalized * attractorStrength);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, attractorRange);
        }
    }
}