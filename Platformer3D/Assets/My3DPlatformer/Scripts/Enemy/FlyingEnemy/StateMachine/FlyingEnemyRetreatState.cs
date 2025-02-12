using UnityEngine;

namespace Platformer
{
    public class FlyingEnemyRetreatState : FlyingEnemyBaseState
    {
        
        public FlyingEnemyRetreatState(FlyingEnemy flyingEnemy, Animator animator, Transform player, float Re) : base(flyingEnemy, animator)
        {
            //Noop
        }

        public override void OnEnter()
        {
            Debug.Log("Enemy is Retreated and will  find a safe place");
        }

        public override void Update()
        {
            
        }
    }
}
