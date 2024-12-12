using UnityEngine;

namespace Platformer
{
    public class GlideState : BaseState {
        public GlideState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter() {
            //animator.CrossFade(Glide, crossFadeDuration);
        }

        public override void FixedUpdate() {
            player.HandleGlide();
            player.HandleMovement();
        }
    }
}
