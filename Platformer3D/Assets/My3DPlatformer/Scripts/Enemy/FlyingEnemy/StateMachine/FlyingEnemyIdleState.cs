using UnityEngine;

namespace Platformer
{
    public class FlyingEnemyIdleState : FlyingEnemyBaseState
    {
        

        public FlyingEnemyIdleState(FlyingEnemy flyingEnemy, Animator animator) : base(flyingEnemy, animator)
        {
            //noop
        }
        
        public override void OnEnter()
        {
            Debug.Log("Idle");
            animator.CrossFade(FlyIdleHash, crossFadeDuration); // Play the idle animation
        }
        public override void Update()
        {
            //noop
        }
    }
}
