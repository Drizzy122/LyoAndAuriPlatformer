using UnityEngine;

namespace Platformer
{
    public class WallClimbState : BaseState
    {
        public WallClimbState(PlayerController player, Animator animator) : base(player, animator)
        {
            
        }

        public override void OnEnter()
        {
            Debug.Log("Player is now entered the Climbing State");
            animator.CrossFade(WallclimbHash, crossFadeDuration);
        }
        public override void Update()
        {
            
        }
    }
}
