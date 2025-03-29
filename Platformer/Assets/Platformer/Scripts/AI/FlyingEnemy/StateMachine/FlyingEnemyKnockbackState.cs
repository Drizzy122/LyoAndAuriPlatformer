using Platformer;
using UnityEngine;

namespace MyNamespace
{
    public class FlyingEnemyKnockbackState : FlyingEnemyBaseState
    {
        readonly Transform player;
        private readonly float speed;
        private readonly float rotationSpeed;

        
        public FlyingEnemyKnockbackState(FlyingEnemy flyingEnemy, Animator animator, Transform player, float speed, float rotationSpeed) : base(flyingEnemy, animator)
        {
            this.player = player;
            this.speed = speed;
            this.rotationSpeed = rotationSpeed;
        }
        
        public override void OnEnter() 
        {
            animator.CrossFade(AttackHash, crossFadeDuration);
        }
        
        public override void Update() 
        {
            Vector3 targetPosition = flyingEnemy.transform.position + (flyingEnemy.transform.position - player.transform.position).normalized * 3;

            // Rotate towards the current waypoint
            RotateTowards(targetPosition);
            
            // Move towards the current waypoint
            flyingEnemy.transform.position = Vector3.MoveTowards(
                flyingEnemy.transform.position, 
                targetPosition, 
                speed * Time.deltaTime
            );
        }
        
        private void RotateTowards(Vector3 target)
        {
            Vector3 direction = (target - flyingEnemy.transform.position).normalized; // Direction to face
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            flyingEnemy.transform.rotation = Quaternion.Slerp(
                flyingEnemy.transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }
    }
}