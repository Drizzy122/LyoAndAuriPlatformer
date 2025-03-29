using Platformer;
using UnityEngine;

namespace MyNamespace
{
    public class FlyingEnemyKnockbackState : FlyingEnemyBaseState
    {
        
        readonly Transform player;
        
        public FlyingEnemyKnockbackState(FlyingEnemy flyingEnemy, Animator animator, Transform player) : base(flyingEnemy, animator)
        {
            this.player = player;
        }
        
        public override void OnEnter() 
        {
            animator.CrossFade(AttackHash, crossFadeDuration);
        }
        
        public override void Update() 
        {
           
        }
    }
}