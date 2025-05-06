using UnityEngine;

namespace Platformer
{
    public class DrownState : BaseState
    {
        public DrownState(PlayerController player, Animator animator) : base(player, animator)
        {
            
        }
        
        public override void OnEnter()
        {
            Debug.Log("Player drowned");
            animator.CrossFade(DrowningHash, crossFadeDuration);
            player.waterParticles.Play();
        }
        public override void FixedUpdate()
        {
            
        }
    }
}