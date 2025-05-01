using UnityEngine;

namespace Platformer
{
    public class LandState : BaseState
    {
        public LandState(PlayerController player, Animator animator) : base(player, animator)
        {
        }
        
        public override void OnEnter()
        {
            animator.CrossFade(LandHash, crossFadeDuration);
        }
        public override void FixedUpdate()
        {
            player.HandleMovement();
        }
    }
}