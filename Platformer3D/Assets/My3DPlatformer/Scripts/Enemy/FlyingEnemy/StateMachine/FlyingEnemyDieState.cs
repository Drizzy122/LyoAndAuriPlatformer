using UnityEngine;

namespace Platformer
{
    
    public class FlyingEnemyDieState : FlyingEnemyBaseState
    {
        
        public FlyingEnemyDieState(FlyingEnemy flyingEnemy, Animator animator) : base(flyingEnemy, animator)
        {
            //Noop
        }
        
        public override void OnEnter()
        {
            //Debug.Log("Enemy Is Dead");
            animator.CrossFade(DieHash, crossFadeDuration);
            
            // Disable enemy colliders or any other relevant components
            flyingEnemy.GetComponent<Collider>().enabled = false;

            // Optionally destroy the enemy after a delay
            GameObject.Destroy(flyingEnemy.gameObject, 3f);

        }

        public override void Update()
        {
            //Noop
        }
    }
}
