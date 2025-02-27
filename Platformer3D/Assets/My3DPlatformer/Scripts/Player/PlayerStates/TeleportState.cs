using UnityEngine;

namespace Platformer
{
    public class TeleportState : BaseState
    {
        public TeleportState(PlayerController player, Animator animator) : base(player, animator) { }
        
        public override void OnEnter() {
            animator.CrossFade(Glide, crossFadeDuration);
        }
        public override void FixedUpdate() 
        {
            
        }
    }
}
