using UnityEngine;

namespace Platformer
{ 
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] float groundDistance = 0.3f;
        [SerializeField] LayerMask groundLayers;

        public bool IsGrounded;

        void Update()
        {
            RaycastHit[] hits = (Physics.SphereCastAll(transform.position + Vector3.up * groundDistance * 0.1f,
                groundDistance, Vector3.down, 0, groundLayers));
            IsGrounded = hits.Length > 0;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * groundDistance * 0.1f, groundDistance);
        }
    }
}