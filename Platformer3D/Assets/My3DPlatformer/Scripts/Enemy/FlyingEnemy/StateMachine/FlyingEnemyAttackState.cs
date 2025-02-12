using UnityEngine;

namespace Platformer
{
    public class FlyingEnemyAttackState : FlyingEnemyBaseState
    {
        readonly Transform player;
        readonly float rotationSpeed;

        public FlyingEnemyAttackState(FlyingEnemy flyingEnemy, Animator animator, Transform player, float rotationSpeed)
            : base(flyingEnemy, animator)
        {
            this.player = player;
            this.rotationSpeed = rotationSpeed;
        }

        public override void OnEnter()
        {
            //Debug.Log("Attack");
            animator.CrossFade(AttackHash, crossFadeDuration);
        }

        public override void Update()
        {
            // Rotate toward the player
            RotateTowards(player.position);
            flyingEnemy.Attack();
            // Additional attack logic if needed
        }

        void RotateTowards(Vector3 target)
        {
            Vector3 direction = (target - flyingEnemy.transform.position).normalized; // Direction to face
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            flyingEnemy.transform.rotation = Quaternion.Slerp(flyingEnemy.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}