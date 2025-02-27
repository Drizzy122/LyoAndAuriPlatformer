using System;
using UnityEngine;

namespace Platformer
{

    public class glideBoostRing : MonoBehaviour
    {
        public float glideBoostAmount = 2;
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerController playerController))
            {
                if (playerController.glideBoost <= 1)
                {
                    playerController.glideBoost = glideBoostAmount;
                }
            }
        }
    }
}
