using UnityEngine;

namespace Platformer
{
    public class FallState : BaseState
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public FallState(PlayerController player, Animator animator) : base(player, animator)
        {
            
        }
        
        public override void OnEnter()
        {
            animator.CrossFade(FallHash, crossFadeDuration);
        }
        public override void FixedUpdate()
        {
            player.HandleMovement();
        }
    }
}